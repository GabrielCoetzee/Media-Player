# Queue Loader Extraction Plan

This document is the agreed plan for extracting the streaming media-load pipeline and the metadata-update pipeline out of `QueueViewModel`, so the VM becomes "just the queue" (items + selection + navigation + the high-level load/enrich workflow). Implemented on branch `feature/visualizations` as part of phase-1 groundwork for the visualizations work.

---

## Context

`QueueViewModel` currently bundles three loosely-related responsibilities under "the queue":

1. **Queue model proper** — `MediaItems`, `SelectedMediaItem`, navigation indices, `RemoveMediaItem`. This is the queue's identity.
2. **Media-loading pipeline** — `AddMediaAsync` with `FlushBatchSize` / `FlushIntervalMs` / `Stopwatch` batching, `_loadMediaTokenSources`, `CancelMediaLoad`. This is a streaming I/O orchestrator that happens to append to the queue.
3. **Metadata-update pipeline** — `UpdateMetadataAsync`, `_updateMetadataTokenSources`, `CancelMetadataUpdate`. Independent lifecycle from loading.

The two pipelines have *identical* cancellation semantics: both maintain a list of in-flight CTSes and expose a `Cancel*` method that "cancels everything"; both are fired by exactly two call sites (`ClearMediaListAsync` and `MessengerRegistrations.SaveChangesToDirtyFiles` at shutdown). Neither ever needs to cancel a specific operation.

Today the queue VM holds both lists and exposes both cancel methods purely so external callers can request "stop everything." The mechanism (batching, CTS bookkeeping, parallel `Task.WhenAll`) is implementation detail that doesn't belong in a view-model.

This refactor extracts the *mechanism* into two services that match the codebase's existing service-layer pattern, while leaving *orchestration* (busy-state copy, messenger broadcasts, "load then enrich") in the queue VM where the workflow's home is.

---

## Goal

`QueueViewModel` after the refactor:

- Holds zero `CancellationTokenSource` state.
- No batching constants, no `Stopwatch`, no `await foreach` over the raw reader.
- No public `CancelMediaLoad` / `CancelMetadataUpdate` methods.
- `AddMediaAsync` shrinks to: busy-state start → foreach over loader batches → busy-state end → call updater → `AutoAdjustAccent` send.
- The two known leaks (`AddMediaCommand` placement, `AutoAdjustAccent` broadcast scattering) are **explicitly deferred** to follow-up changes — see "Known Issues Carried Forward" below.

---

## Decisions Locked

| # | Decision |
|---|---|
| 1 | **New service `IMediaLoader` in `MediaPlayer.ViewModel/Services/Abstract`** with concrete `MediaLoader` in `Services/Concrete`. It is *not* added to the `IMetadataServices` aggregator — it's a streaming-load coordinator, not a metadata service. `QueueViewModel` imports it as a standalone `[Import]`. |
| 2 | **Loader interface:** `IAsyncEnumerable<IReadOnlyList<MediaItem>> LoadInBatchesAsync(IEnumerable<string> paths)` plus `void Cancel()`. No `CancellationToken` parameter — the loader owns its own. |
| 3 | **Loader is stateful.** It maintains an internal `List<CancellationTokenSource>` (one CTS per in-flight call, mirroring today's queue-VM pattern). `Cancel()` cancels all in-flight tokens and removes them from the list. Subsequent `LoadInBatchesAsync` calls create fresh CTSes — the service is re-callable after cancel. |
| 4 | **Loader propagates `OperationCanceledException`.** Matches the standard .NET cancellation idiom. Callers wrap the `await foreach` in `try { ... } catch (OperationCanceledException) { return; }`. |
| 5 | **Loader is pure mechanism.** It does NOT import `BusyViewModel`, does NOT send messenger broadcasts, does NOT call `IMetadataUpdateService`. Busy-state copy, messenger sends, and "then enrich" all remain in `QueueViewModel.AddMediaAsync`. |
| 6 | **Loader discards partial batches on cancel** (preserves today's behavior). The loader's internal try block catches its own cancellation, discards `pendingItems`, then rethrows. Consumers never see the partial batch. |
| 7 | **`IMetadataUpdateService` gains symmetric treatment.** Its `UpdateMetadataAsync(items, CancellationToken token)` signature loses the token parameter. The service maintains its own internal CTS list and exposes `void Cancel()`. |
| 8 | **`IMetadataUpdateService` propagates `OperationCanceledException`** but commits partial enrichments first. The existing `try/catch (OperationCanceledException)` swallows inside `UpdateAlbumArtAsync` / `UpdateLyricsAsync` are removed. The post-await `EnrichLyrics` / `EnrichAlbumArt` loop moves into a `finally` so partial results are committed even on cancel — matching today's de-facto behavior. The `Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent)` at the end of `UpdateAlbumArtAsync` also moves into the `finally`. |
| 9 | **`QueueViewModel` drops `_loadMediaTokenSources`, `_updateMetadataTokenSources`, `CancelMediaLoad()`, `CancelMetadataUpdate()`, `FlushBatchSize`, `FlushIntervalMs`, and the `Stopwatch` usage.** |
| 10 | **`MessengerRegistrations.SaveChangesToDirtyFiles` reaches through to the services directly.** It resolves `IMediaLoader` and `IMetadataUpdateService` from the container and calls `.Cancel()` on each. The thin forwarder methods on `QueueViewModel` are removed, not preserved. |
| 11 | **`ClearMediaListAsync` (inside `QueueViewModel`) calls `MediaLoader.Cancel()` and `MetadataServices.MetadataUpdater.Cancel()` directly** — no internal forwarder methods. |

---

## Implementation Steps

### 1. Create `IMediaLoader` and `MediaLoader`

- New file: `MediaPlayer.ViewModel/Services/Abstract/IMediaLoader.cs`
- New file: `MediaPlayer.ViewModel/Services/Concrete/MediaLoader.cs`
- `MediaLoader` imports `IMetadataServices` (or just `IMetadataReaderService` if cleaner — to be decided during impl) and wraps `MetadataReader.EnumerateMediaItemsAsync` with batching logic moved verbatim from `QueueViewModel.AddMediaAsync` (lines 80–110).
- Owns `FlushBatchSize = 25`, `FlushIntervalMs = 150`, the per-call `Stopwatch`, the `pendingItems` list, and an internal `List<CancellationTokenSource>`.
- `Cancel()` iterates the CTS list and calls `.Cancel()` on each. CTS list cleanup happens in the per-call `finally`.
- `[Export(typeof(IMediaLoader))]` — singleton, matches existing service registration pattern.

### 2. Augment `IMetadataUpdateService`

- Change signature: `Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems)` (drop the `CancellationToken` parameter).
- Add: `void Cancel()`.
- In `MetadataUpdateService`:
  - Add internal `List<CancellationTokenSource>` field.
  - `UpdateMetadataAsync` creates its own CTS, adds to list, passes the token down to `UpdateAlbumArtAsync` / `UpdateLyricsAsync`, removes + disposes in `finally`.
  - Remove the internal `try/catch (OperationCanceledException)` and `catch (TaskCanceledException)` swallows in both `UpdateAlbumArtAsync` and `UpdateLyricsAsync`.
  - Move the post-await `Enrich*` calls and the `AutoAdjustAccent` send into `finally` blocks so partial results commit on cancel.

### 3. Update `QueueViewModel`

- Add `[Import] public IMediaLoader MediaLoader { get; set; }`.
- Delete fields: `FlushBatchSize`, `FlushIntervalMs`, `_loadMediaTokenSources`, `_updateMetadataTokenSources`.
- Delete methods: `CancelMediaLoad()`, `CancelMetadataUpdate()`.
- Rewrite `AddMediaAsync`:

  ```csharp
  public async Task AddMediaAsync(IEnumerable<string> paths)
  {
      if (paths == null || !paths.Any())
          return;

      BusyViewModel.MediaListLoading();

      var newlyAddedItems = new List<MediaItem>();

      try
      {
          await foreach (var batch in MediaLoader.LoadInBatchesAsync(paths))
          {
              newlyAddedItems.AddRange(batch);
              AddMediaItemsToListView(batch);
          }
      }
      catch (OperationCanceledException)
      {
          return;
      }

      BusyViewModel.MediaListPopulated();

      await UpdateMetadataAsync(newlyAddedItems.OfType<AudioItem>());

      Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
  }
  ```

- Rewrite `UpdateMetadataAsync` (now much smaller):

  ```csharp
  private async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems)
  {
      if (!SettingsViewModel.UpdateMetadata || !audioItems.Any())
          return;

      BusyViewModel.UpdatingMetadata();

      try
      {
          await MetadataServices.MetadataUpdater.UpdateMetadataAsync(audioItems);

          BusyViewModel.MediaListPopulated();

          MetadataServices.MetadataCorrector.FixMetadata(audioItems);
      }
      catch (OperationCanceledException)
      {
          // partial enrichments already committed by the service; nothing else to do
      }
  }
  ```

- `AddMediaItemsToListView` keeps its existing shape (`MediaItems.AddRange` + first-batch selection + `CommandManager.InvalidateRequerySuggested()`); it now accepts `IReadOnlyList<MediaItem>` instead of `IEnumerable<MediaItem>` for clarity.
- Update `ClearMediaListAsync` to call `MediaLoader.Cancel()` and `MetadataServices.MetadataUpdater.Cancel()` directly instead of the deleted forwarder methods.

### 4. Update `MessengerRegistrations.SaveChangesToDirtyFiles`

```csharp
public static void SaveChangesToDirtyFiles(CompositionContainer container)
{
    Messenger<MessengerMessages, ShutdownArgs>.Register(MessengerMessages.SaveChangesToDirtyFiles, async (args) =>
    {
        var loader = container?.GetExportedValue<IMediaLoader>();
        var updater = container?.GetExportedValue<IMetadataUpdateService>();
        var queue = container?.GetExportedValue<QueueViewModel>();

        loader.Cancel();
        updater.Cancel();

        await queue.SaveDirtyMetadataAsync();

        if (args.IsEnabled)
            Application.Current.Shutdown(0);
    });
}
```

### 5. Update tests

- `QueueViewModelTests`:
  - Replace the `IMetadataReaderService.EnumerateMediaItemsAsync` mock setups with mocks on a new `Mock<IMediaLoader>` and its `LoadInBatchesAsync` method.
  - The existing `_metadataUpdaterMock` setups for `UpdateMetadataAsync` need their signature updated (drop the `CancellationToken` parameter).
  - The `AddMediaAsync_LoadCancelledMidStream_SwallowsCancellationInsteadOfThrowing` test continues to assert "no exception bubbles out of `AddMediaAsync`" — but now the mocked `IMediaLoader` throws `OperationCanceledException` from its `IAsyncEnumerable`, rather than the reader doing so via a cancellation token.
- New `MediaLoaderTests` covering: batching thresholds, cancel-mid-stream propagation, partial-batch discard, re-callability after cancel.
- Existing `MetadataReaderServiceTests` continue to test `EnumerateMediaItemsAsync` directly — no change needed there.

### 6. Update `CLAUDE.md`

The "ViewModel Structure" section's description of `QueueViewModel` mentions "the metadata pipeline (`AddMediaAsync`, `UpdateMetadataAsync`)" — this stays accurate (the VM still orchestrates the workflow), but a sentence should be added noting that the *mechanism* (batching, cancellation) lives in `IMediaLoader` and `IMetadataUpdateService`.

---

## Known Issues Carried Forward

These are real but **explicitly out of scope for this refactor**. They have their own follow-up changes to address right after this one merges.

1. **`AddMediaCommand` lives on `MediaControlsViewModel` instead of `QueueViewModel`.** Conceptually a queue op, hosted on the controls VM because the transport bar binds it. The fix is to relocate the `[Import]` to `QueueViewModel` and update the XAML binding path on the transport bar. Small, focused change.

2. **`AutoAdjustAccent` broadcasts have no clear owner.** After this refactor, four senders remain (queue setter, end of `AddMediaAsync`, end of `UpdateAlbumArtAsync`, theme property setters), and `ThemeViewModel` is both a sender *and* the receiver across a global messenger bus. The clean fix is for `ThemeViewModel` to subscribe to `QueueViewModel.PropertyChanged` for `SelectedMediaItem` (and to album-art change notifications) directly, so the broadcast goes from 4 senders to 0 — receivers react. This is its own design conversation and its own implementation pass.

---

## Out of Scope

- Moving `MetadataUpdateService` to `MediaPlayer.Model` (it mutates `AudioItem` via `EnrichLyrics` / `EnrichAlbumArt`, which would put it under the project's service-vs-model rule, but that's a separate placement decision).
- Touching `BusyViewModel`'s canned-method API shape.
- Any visualization work.

---

## Test Plan

- `dotnet build src/MediaPlayer.sln` → 0 warnings, 0 errors.
- `dotnet test src/MediaPlayer.ViewModel.Test` → all existing queue + updater tests pass (with signature-mocking updates), new `MediaLoaderTests` pass.
- Manual smoke: drop files onto the player, confirm queue populates incrementally as before; clear the list mid-load, confirm no exception; close the app mid-load, confirm clean shutdown.
