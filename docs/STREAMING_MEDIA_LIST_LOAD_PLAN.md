# Streaming media-list load (items appear as they're processed)

## Context

Today, dropping a folder into the main window blocks the UI until **every** file has
been enumerated and read by TagLib#. Only then does the queue populate (single
`CollectionChanged.Reset`) and the first track start playing. For a large music folder
this is a noticeable dead wait with no feedback.

Goal: items appear in the queue incrementally as they're built, the first track starts
playing as soon as the first item arrives, and the rest of the list fills in behind it.
The slow network metadata enrichment (Last.fm art, LyricsOVH) stays as one batch that
kicks off once the full set of audio items is known (decision: keep it simple, no
regression vs. today, busy indicator stays clean).

This is a contained change: two production files plus their tests. The domain model, DI
wiring, messaging, and the `ShuffleCommand`/`ClearMediaListCommand` paths are untouched —
they keep using `MediaItemObservableCollection.AddRange`.

## Approach: `IAsyncEnumerable<MediaItem>` + chunk-flush on the consumer

### 1. `MetadataReaderService` — stream items instead of returning a list

- `IMetadataReaderService.EnumerateMediaItemsAsync` signature →
  `IAsyncEnumerable<MediaItem> EnumerateMediaItemsAsync(IEnumerable<string> paths, CancellationToken ct = default)`.
- Impl: `async IAsyncEnumerable<MediaItem>` (`[EnumeratorCancellation]` on the token) that
  iterates the existing `SearchFolders(...)` then `SearchFiles(...)` sequences, and for
  each path does `var item = await Task.Run(() => _metadataReader.BuildMediaItem(file), ct);`
  then `if (item != null) yield return item;`. Wrapping each `BuildMediaItem` in `Task.Run`
  keeps the synchronous TagLib# read off the UI thread; the `await` resumes on the captured
  UI `SynchronizationContext`.
- Null filtering (corrupt files) moves inline, replacing the trailing `.Where(x => x != null)`.

### 2. `MainViewModel` — consume the stream, flush in small chunks

Rework `AddMediaAsync`:

- `BusyViewModel.MediaListLoading()` as today.
- `await foreach` over the stream; accumulate every item into `allItems` and a `pending`
  buffer. When `pending.Count >= FlushBatchSize` (25) or `Stopwatch.ElapsedMilliseconds >=
  FlushIntervalMs` (150), call `AddMediaItemsToListView(pending)`, clear, restart stopwatch.
- After the loop, flush any remaining `pending` (handles tiny folders — one flush at the end).
- `BusyViewModel.MediaListPopulated()`.
- `await UpdateMetadataAsync(allItems.OfType<AudioItem>())` — single batch, unchanged.
- `Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent)`.

`AddMediaItemsToListView` keeps its current shape (still uses `MediaItems.AddRange` → one
`Reset` per chunk; cheap, ListView handles it, avoids per-item index bookkeeping). The
"first item" bootstrap (`SelectMediaItem` + `SetPlaybackState(Play)`) only fires on the
first chunk thanks to the existing `if (SelectedMediaItem != null) return;` guard.

`FlushBatchSize` / `FlushIntervalMs` as private consts on `MainViewModel`.

**No change** to `MediaItemObservableCollection` or `BulkObservableCollection`.

### 3. Cancellation — stop an in-flight load when the list is cleared

A streaming load can be interrupted by the user (e.g. "Clear all" mid-load). `MainViewModel`
holds a `List<CancellationTokenSource> _loadMediaTokenSources` (mirrors the existing
`_updateMetadataTokenSources`). `AddMediaAsync` creates a CTS, passes its token to
`EnumerateMediaItemsAsync` (which already does `ThrowIfCancellationRequested()` per file),
and removes/disposes it in a `finally`. An `OperationCanceledException` out of the loop is
caught and the method returns without flushing the partial batch / updating metadata /
re-applying the accent.

`MainViewModel.CancelMediaLoad()` cancels all in-flight loads. It's called from
`ReleaseResources()` (alongside `CancelMetadataUpdate()`), which `SaveChangesAsync()` runs
unconditionally — so it's reached on both app shutdown and "Clear all"
(`ClearMediaListCommand.Execute` → `SaveChangesAsync()`) while a load is streaming.

## Files changed

| File | Change |
|---|---|
| `src/MediaPlayer.ViewModel/Services/Abstract/IMetadataReaderService.cs` | signature → `IAsyncEnumerable<MediaItem> EnumerateMediaItemsAsync(IEnumerable<string>, CancellationToken)` |
| `src/MediaPlayer.ViewModel/Services/Concrete/MetadataReaderService.cs` | `async IAsyncEnumerable`, `Task.Run` per `BuildMediaItem`, inline null filter |
| `src/MediaPlayer.ViewModel/ViewModels/MainViewModel.cs` | `AddMediaAsync` consumes stream with chunk-flush; `CancelMediaLoad()`; cancellation token threaded through |
| `src/MediaPlayer.ViewModel/Commands/Concrete/ClearMediaListCommand.cs` | calls `vm.CancelMediaLoad()` before clearing |

## Tests updated

- `MainViewModelTests.cs`: the two `_metadataReaderMock.Setup(x => x.EnumerateMediaItemsAsync(...))`
  `.ReturnsAsync(...)` calls return an async-stream helper instead. Existing assertions unchanged.
- `MetadataReaderServiceTests.cs`: materialize the stream via `await foreach` into a list, then
  keep the existing count/content assertions.

## Verification

1. `dotnet build src/MediaPlayer.sln` — clean.
2. `dotnet test src/MediaPlayer.ViewModel.Test` — all green.
3. `dotnet run --project src/MediaPlayer.Shell`:
   - Large folder: queue rows appear within a moment and keep growing; first track plays
     well before the full list is in; art/lyrics fill in after the scan finishes.
   - Single file / tiny folder: behaves as before (one flush, first item selected + playing).
   - Shuffle / Clear / Next-Track while still streaming: no exceptions; commands light up
     once the first chunk lands.
   - Folder with a corrupt/zero-byte audio file: silently skipped, no crash.
