# Audio Engine Queue Removal Plan

This document is the agreed plan for removing the engine-side playlist from `MediaPlayer.AudioEngine` and making `MainViewModel.MediaItems` the single source of truth for the queue. Implemented on branch `feature/visualizations` alongside the in-progress LibVLC engine extraction.

---

## Context

`MediaPlayer.AudioEngine` (introduced on this branch) currently exposes a playlist-aware engine: it holds `_paths` and `_currentTrackIndex`, advances tracks internally on `EndReached`, and surfaces `LoadPlaylist` / `AppendToPlaylist` / `RemoveFromPlaylist` / `ClearPlaylist` / `PlayAt(int)` / `NextTrack` / `PreviousTrack`. The orchestrator (`MainViewModel`) also owns the queue via `MediaItems`. Two parallel collections, manually kept in sync via `ResyncEnginePlaylist()` called from every `MediaItems.CollectionChanged` event.

Findings from grilling the design:

- `BulkObservableCollection.AddRange` raises a single `Reset` event, **not** per-item `Add`. The same applies to `RemoveRange`. So per-action dispatch on `e.Action` would optimize only the single-item `Remove` path and the rare `Move`.
- Even with per-action dispatch, the engine and orchestrator each carry their own representation of "what's in the queue" — a duplication that produces sync bugs.
- Concrete latent bug: when items before the currently-playing track are removed, the engine's `_currentTrackIndex` does not shift down. The next `EndReached` advances to the wrong track.
- `LoadPlaylist`'s current implementation does not interrupt playback (it only swaps `_paths`; `_mediaPlayer.Media` is untouched), so today's blanket resync is *safe* but wasteful and conceptually misleading.

The root cause is that the engine straddles two roles — *audio decoder* and *queue manager*. Splitting them removes the entire class of sync bugs.

---

## Goal

Delete the engine-side playlist entirely. The engine becomes a single-file decoder. `MainViewModel.MediaItems` is the only queue. Queue navigation lives in the existing commands (`NextTrackCommand`, `PreviousTrackCommand`), and `MainViewModel`'s "track ended" handler delegates to `NextTrackCommand` with a `Stop()` fallback when neither a next track nor repeat applies.

---

## Decisions Locked

| # | Decision |
|---|---|
| 1 | **Engine becomes queue-unaware.** Drop `_paths`, `_currentTrackIndex`, and queue methods: `LoadPlaylist`, `AppendToPlaylist`, `RemoveFromPlaylist`, `ClearPlaylist`, `PlayAt(int)`, `NextTrack`, `PreviousTrack`. |
| 2 | **New engine surface.** Methods: `Play(string path)`, `TogglePause()`, `Stop()`, `SeekTo(TimeSpan)`. Properties: `Position`, `Duration`, `Volume`, `PlaybackState`, `CurrentTrackPath`, `NativePlayer`. |
| 3 | **Engine event renames.** `TrackChanged` deleted (the orchestrator already knows what it asked to play). Replaced by a focused `DurationDiscovered` event for the only piece the orchestrator still needs from the old event — VLC's asynchronously-reported track length. `Ended` renamed to `TrackEnded` and now fires on every track's natural end, not "playlist exhausted." |
| 4 | **Drop `PlaybackState.Ended`.** Engine only knows `Playing` / `Paused` / `Stopped`. The "Ended" concept was a leak from the queue-manager role. |
| 5 | **`MediaControlsViewModel` exposes engine-shaped primitives.** `Play(string)`, `TogglePause()`, `Stop()`. The old `TogglePlayPause()` method is replaced. `IsPlaying` / `HasLoadedMedia` flags were initially planned to support a "fallback play if nothing loaded" branch in `PlayPauseCommand`, but the fallback turned out to be unreachable (see post-implementation note) and the flags were stripped along with it. |
| 6 | **Queue navigation stays in `MainViewModel` + commands.** `NextTrackCommand` and `PreviousTrackCommand` already compute the right index (with repeat-wrap). The `Ended` handler delegates to `NextTrackCommand`, falling back to `Stop()` when `CanExecute` returns false. No new commands needed. |
| 7 | **Stale-event defense: path-stamp + skip-zero filter.** `DurationDiscoveredEventArgs` and `TrackEndedEventArgs` carry `Path`. Orchestrator drops events whose path doesn't match `SelectedMediaItem.FilePath.LocalPath`. Engine drops `LengthChanged` callbacks with `e.Length <= 0` to avoid the "transient 0 duration" blink during media switches. No token correlation — race is rare in practice and self-corrects when VLC parses the new media. |
| 8 | **Threading: keep status quo.** Engine write methods still queue to `ThreadPool.QueueUserWorkItem`. The defensive dispatch absorbs cold-disk file-open latency. With the queue methods gone, the race surface shrinks naturally; remaining races are addressed by Decision 7. |
| 9 | **`Stop()` preserves `CurrentTrackPath`.** Matches today's behavior — clicking Play after Stop restarts the same track from the beginning via `TogglePause()` (which falls through to `_mediaPlayer.Play()` from the `Stopped` state). |
| 10 | **`NotifyEngineOfSelection` becomes path-shaped.** Setting `SelectedMediaItem` no longer computes an index — it extracts `item.FilePath?.LocalPath` and calls `MediaControlsViewModel.Play(path)`. |
| 11 | **Initial sync removed.** `ResyncEnginePlaylist()` and its call from `OnImportsSatisfied` are deleted — there is no engine-side playlist to seed. |

---

## Target `IAudioEngine`

```csharp
public interface IAudioEngine
{
    TimeSpan Position { get; }
    TimeSpan Duration { get; }
    double Volume { get; set; }
    PlaybackState PlaybackState { get; }
    string CurrentTrackPath { get; }
    VlcMediaPlayer NativePlayer { get; }   // load-bearing: HeroArea.xaml VideoView binding

    event EventHandler<PlaybackPositionChangedEventArgs> PositionChanged;
    event EventHandler<PlaybackStateChangedEventArgs>    StateChanged;
    event EventHandler<DurationDiscoveredEventArgs>      DurationDiscovered;
    event EventHandler<TrackEndedEventArgs>              TrackEnded;

    void Play(string path);   // load + start a new file
    void TogglePause();       // pause if playing, resume/restart otherwise
    void Stop();
    void SeekTo(TimeSpan position);
}
```

`PlaybackState` enum: `Playing`, `Paused`, `Stopped` (no `Ended`).

`DurationDiscoveredEventArgs`: `string Path`, `TimeSpan Duration`.
`TrackEndedEventArgs`: `string Path`.

---

## File-by-File Impact

### `MediaPlayer.AudioEngine`

1. **`Abstract/IAudioEngine.cs`** — replace with the slim interface above.
2. **`Concrete/LibVlcAudioEngine.cs`** — remove `_paths`, `_currentTrackIndex`, `_lock` (no shared collection to guard); replace with a single `_currentTrackPath` field; implement `Play(path)` (load + play); implement `TogglePause()` (state-dependent dispatch); rewrite `OnInnerEndReached` to just raise `TrackEnded` and let the orchestrator decide what's next; rewrite `OnInnerLengthChanged` to raise `DurationDiscovered` with skip-zero filter; delete `LoadAndPlay`, `PlayInternal`'s playlist fallback, `NextTrack`, `PreviousTrack`, `LoadPlaylist`, `AppendToPlaylist`, `RemoveFromPlaylist`, `ClearPlaylist`, `PlayAt`.
3. **`Events/TrackChangedEventArgs.cs`** — DELETE.
4. **`Events/DurationDiscoveredEventArgs.cs`** — NEW.
5. **`Events/PlaybackEndedEventArgs.cs`** — replaced by `Events/TrackEndedEventArgs.cs` (path-stamped).
6. **`Enumerations/PlaybackState.cs`** — remove `Ended`.

### `MediaPlayer.ViewModel`

7. **`ViewModels/MediaControlsViewModel.cs`** — replace `TogglePlayPause()` and `PlayAt(int)` with `Play(string path)` and `TogglePause()`; re-raise `DurationDiscovered` and `TrackEnded`; drop `TrackChanged` re-raise.
8. **`ViewModels/MainViewModel.cs`** — delete `ResyncEnginePlaylist()`; in `MediaItems_CollectionChanged`, keep only the `OnPropertyChanged(nameof(IsMediaListPopulated))` line; replace `MediaControlsViewModel_TrackChanged` handler with `MediaControlsViewModel_DurationDiscovered` (path-validated against `SelectedMediaItem.FilePath.LocalPath`); rewrite `MediaControlsViewModel_Ended` to delegate to `NextTrackCommand` (`CanExecute` then `Execute`, else `Stop()`); rewrite `NotifyEngineOfSelection` to call `MediaControlsViewModel.Play(path)`; remove the `ResyncEnginePlaylist()` call from `OnImportsSatisfied`.
9. **`Commands/Concrete/PlayPauseCommand.cs`** — call `MediaControlsViewModel.TogglePause()` unconditionally. (An initial draft included a "load fallback if nothing playing" branch; discovered to be dead code — see post-implementation note.)

### Tests

10. **`MediaPlayer.ViewModel.Test/ViewModelTests/MediaControlsViewModelTests.cs`** — remove the two `[TestCase(PlaybackState.Ended)]` cases; replace `SetupGet(x => x.CurrentTrackIndex)` setups with `SetupGet(x => x.CurrentTrackPath)`; delete `EngineTrackChanged_ReRaisesOnViewModel`; add `EngineDurationDiscovered_ReRaisesOnViewModel`; update the `Ended` re-raise test to use `TrackEndedEventArgs`.
11. **`MediaPlayer.ViewModel.Test/ViewModelTests/MainViewModelTests.cs`** — review tests that interact with `MediaControlsViewModel`'s old surface (`PlayAt`, `Stop`); none should assert engine-sync behavior since `ResyncEnginePlaylist` was private. Drop any assertions that touch `TrackChanged`.

---

## Behavioural Equivalences (sanity-check)

Verify the new flow preserves today's user-visible behavior:

- **Mid-queue track end** — today: engine auto-advances internally. After D: `TrackEnded` → `NextTrackCommand.CanExecute` true (`IsNextMediaItemAvailable`) → advances. ✓
- **Last track end, repeat off** — today: engine sets `Ended` state, orchestrator's `Ended` handler calls `Stop()`. After D: `TrackEnded` → `NextTrackCommand.CanExecute` false → `Stop()`. ✓
- **Last track end, repeat on** — today: orchestrator's `Ended` handler selects first item. After D: `TrackEnded` → `NextTrackCommand.CanExecute` true (`IsRepeatEnabled`) → `PlayNextMediaItem` wraps to first. ✓
- **User clicks Stop button** — today: `StopCommand` selects first item then calls `Stop()`. After D: identical (`SelectMediaItem(0)` → `Play(path)` → `Stop()`). ✓
- **User clicks Play after queue ended** — today: `engine.Play()` finds `_mediaPlayer.Media != null` (last track) and restarts it. After D: `TogglePause()` from `Stopped` state calls `_mediaPlayer.Play()` which restarts the last loaded track. ✓
- **User clicks Play with nothing playing and SelectedMediaItem set** — handled by the same `TogglePause()` path. `_currentTrackPath` was set the moment auto-selection issued `Play(path)`; `TogglePause()` from the resulting `Paused` / `Stopped` state resumes/restarts. ✓

---

## Risks

- **Duration-discovery race window.** If the user clicks two different tracks within VLC's metadata-parse window (typically <50 ms for audio, longer for video), a `LengthChanged` callback for the old track could surface stamped with the new track's path. The defenses chosen (path-stamp, skip-zero) handle the common case; the residual race is extremely rare and self-corrects within one parse cycle. If observed in practice, the fix is engine-local: introduce a token incremented per `Play(path)` and gate event raises on token match.
- **`NativePlayer` binding survival.** Verified: `HeroArea.xaml:24` binds `MediaPlayer="{Binding AudioEngine.NativePlayer}"`. The property remains on `IAudioEngine`.

---

## Workflow

- Lands on `feature/visualizations` (same branch as the engine extraction).
- Implement → `dotnet build src/MediaPlayer.sln` → `dotnet test src/MediaPlayer.ViewModel.Test`.
- Leave uncommitted for local review. No commit/PR without explicit sign-off.

---

## Post-implementation Notes

### `PlayPauseCommand` fallback was dead code

The initial draft of Decision 5 / file #9 included a fallback in `PlayPauseCommand.Execute`: if neither `IsPlaying` nor `HasLoadedMedia` were true, the command would set `SelectedMediaItem = SelectedMediaItem ?? MediaItems.FirstOrDefault()` to trigger playback via the setter side-effect. This was supposed to replicate the old engine's `_paths[0]` fallback in `PlayInternal` for the "click Play with nothing loaded" case.

After implementation, the branch was discovered to be unreachable in practice:

- `PlayPauseCommand.CanExecute` already gates on `IsMediaListPopulated`, so the button is disabled until items exist in `MediaItems`.
- `MainViewModel.AddMediaItemsToListView` auto-selects the first item when `MediaItems` transitions from empty → populated and `SelectedMediaItem` is null. The selection setter calls `MediaControlsViewModel.Play(path)`, which sets the engine's `_currentTrackPath`. After that, `HasLoadedMedia` is true for the rest of the process lifetime (the engine never clears the path).
- The only theoretical gap is the few microseconds between `MediaItems.AddRange` returning and the `Play(path)` ThreadPool task setting `_currentTrackPath` — too short for a user to click through.

Consequence: the fallback was stripped, and `IsPlaying` / `HasLoadedMedia` (whose sole consumer was the fallback condition) were stripped with it. `PlayPauseCommand.Execute` is now an unconditional `TogglePause()` forward. Final `MediaControlsViewModel` surface is correspondingly smaller than what Decision 5 originally specified.
