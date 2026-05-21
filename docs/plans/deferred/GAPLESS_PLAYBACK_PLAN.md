# Gapless Playback Plan

This document is the agreed plan for adding **gapless playback** (gap-elimination)
to the LibVLC-backed audio engine. It reflects the design grilling that followed the
initial question "how could we achieve gapless playback?"; every branch of the
decision tree was resolved against the actual code and is captured in the Decisions
Locked table.

**Relationship to `VISUALIZATIONS_PLAN.md`:** that plan's decision #3 scoped gapless
as "via LibVLCSharp's `MediaListPlayer`". This plan **supersedes that approach** — a
`MediaListPlayer` reintroduces a list/index-shaped API onto `IAudioEngine`, which
`CLAUDE.md` explicitly bans (the engine had one and it was deliberately removed). The
dual-player + `Preload` design below delivers the same listener-perceived result while
keeping the engine single-file-shaped and the queue/order authority in `QueueViewModel`.

---

## Context

Today every track change cold-loads a new LibVLC `Media` on a single `VlcMediaPlayer`
(`LibVlcAudioEngine.PlayCore`, lines 128-146): old media disposed, new `Media`
created, `Play()` called. The file-open + demux + prebuffer + decoder-start cost
produces an audible ~0.5–2s silence between consecutive tracks. The goal is to
**eliminate that audible pause** between consecutive *audio* tracks while keeping
the LibVLC backend. This is *gap-elimination*, not sample-accurate gapless (no
LAME/iTunSMPB padding handling — a continuous live album will still have a
near-imperceptible seam) and not crossfade.

---

## Decisions Locked

| # | Decision |
|---|----------|
| 1 | **Standard:** gap-elimination (no audible pause), stay on LibVLC. Not sample-accurate, not crossfade. |
| 2 | **Mechanism:** dual `VlcMediaPlayer`; a second player pre-buffers the next track. `IAudioEngine` gains single-file-shaped `Preload(string path)` / `ClearPreload()` — **not** a list/index API, so the `CLAUDE.md` ban on `LoadPlaylist`/`PlayAt` holds. |
| 3 | **Hand-off:** audio leads. On its own `EndReached` the engine swaps to the primed player *instantly* and sets `_currentTrackPath` to the primed path; the existing `MediaControlsViewModel.Play()` guard (`if (path == AudioEngine.CurrentTrackPath) return;`, line 144) absorbs the follow-up `SelectedMediaItemChanged → Play()` as a no-op. |
| 4 | **Timing:** eager — prime the computed next track when the current track starts (required so manual Next is always gapless). |
| 5 | **Consistency:** one shared next-policy used by both the prime computation and `NextTrackCommand`; the `Play()` guard is the self-correction backstop if the queue mutates in-window. |
| 6 | **Manual skip:** manual Next is also gapless (engine promotes the primed player when `Play(path)` matches the primed path). Prev / arbitrary click cold-load as today. |
| 7 | **Rollout:** new `PlaybackSettings.GaplessPlayback`, default **ON**, with a settings toggle (escape hatch for the unverified VLC behaviour below). |
| 8 | **Video:** audio-only. If current *or* next item is a `VideoItem`, no priming/swap — cold-load path. `NativePlayer` (bound by `HeroArea.xaml:24` to the WPF `VideoView`) stays a **fixed primary player**; video always cold-loads on the primary; audio gapless alternates which player is *audio-active* but never touches the video binding. |
| 9 | **Setting home:** new `PlaybackSettings : SerializableSettings` class. |

---

## Design

### Engine (`LibVlcAudioEngine`)
- Hold two players: `_primary` (the one `NativePlayer` always returns) and a
  `_secondary`. Track `_activePlayer` (audio-active) and `_preloadedPath`.
  Subscribe VLC events on both; route to public events **only** for `_activePlayer`
  (suppress the priming player's State/Time/Length/End while it is parked).
- `Preload(path)`: off-UI-thread; no-op if `path == _preloadedPath`. Create `Media`
  on the non-active player, `Play()` then immediately `SetPause(true)` (then
  `SeekTo(TimeSpan.Zero)` if VLC does not park at frame 0 — verify), record
  `_preloadedPath`.
- `ClearPreload()`: stop/release the primed player's media, clear `_preloadedPath`.
- `PromotePreloaded()` (the single shared swap primitive): swap active/preload
  references, set `_currentTrackPath = _preloadedPath`, apply current `Volume` to the
  newly-active player, un-pause it, **re-emit** `StateChanged(Playing)`,
  `DurationDiscovered(activeLength)` and a zero `PositionChanged` (see assumption #3),
  stop+release the old active's media (it becomes the idle preload slot).
- `OnVlcEndReached` (active player): if `_preloadedPath` set → `PromotePreloaded()`
  then fire `TrackEnded(newPath)`; else existing behaviour.
- `Play(path)`: `path == _currentTrackPath` → return (unchanged). Else
  `path == _preloadedPath` → `PromotePreloaded()` (manual-Next gapless). Else cold-load
  (the primary for video, the active player for audio) and `ClearPreload()`.
- **Invariant:** a `VideoItem` always plays on `_primary`; `NativePlayer => _primary`
  always, so the XAML `VideoView` binding never dangles.

### Shared next-policy
- Add `QueueViewModel.PeekNextItem(bool repeatEnabled)` → returns the `MediaItem`
  that would play next, or `null` at end-of-queue with no repeat. Encapsulates
  `GetNextMediaItemIndex()` + the repeat-wrap currently duplicated in
  `NextTrackCommand.PlayNextMediaItem` (lines 37-40).
- Refactor `NextTrackCommand.PlayNextMediaItem` to call `PeekNextItem` and select it
  (removes the duplication; SRP).

### Orchestrator (`MediaControlsViewModel`)
- Re-prime triggers: it already subscribes `QueueViewModel.SelectedMediaItemChanged`;
  also subscribe `QueueViewModel.MediaItems.CollectionChanged` (shuffle/add/remove/clear
  raise a single `Reset` via the bulk collection — no diff, just recompute) and react
  to its own `IsRepeatEnabled` change.
- `RePrime()`: compute `QueueViewModel.PeekNextItem(IsRepeatEnabled)`. Prime via
  `AudioEngine.Preload(nextPath)` **only if** `PlaybackSettings.GaplessPlayback` is on
  **and** both `SelectedMediaItem` and the next item are `AudioItem`; otherwise
  `AudioEngine.ClearPreload()`.
- Import the new `PlaybackSettings` (precedent: `QueueViewModel` imports
  `SettingsViewModel`).

### Settings + toggle
- New `MediaPlayer.Settings/Configuration/PlaybackSettings.cs`: `SerializableSettings`,
  `[InheritedExport][PartCreationPolicy(Shared)]`, `bool GaplessPlayback { get; set; } = true;`,
  `FileName => "Playback Settings"`, `UseEncryption => false`, `Save()` — mirrors
  `MetadataSettings`.
- `SettingsViewModel`: inject `PlaybackSettings` in the `[ImportingConstructor]`; expose
  `GaplessPlayback` get/set (set → settings, `OnPropertyChanged`, `Save()`,
  `Messenger<MessengerMessages>.Send(GaplessPlaybackChanged)`) — mirrors `UpdateMetadata`
  and the `ThemeViewModel` Messenger-on-change precedent.
- Add `MessengerMessages.GaplessPlaybackChanged`; wire a registration in
  `MediaPlayer.Shell/Messenger Registrations/MessengerRegistrations.cs` so toggling
  triggers an immediate `RePrime()`/`ClearPreload()` (handles toggle-off mid-track).
- Add a bound `ToggleSwitch` to the settings panel XAML beside the existing
  `SettingsViewModel.UpdateMetadata` toggle.

---

## Files

- `MediaPlayer.AudioEngine/Abstract/IAudioEngine.cs` — add `Preload`, `ClearPreload`.
- `MediaPlayer.AudioEngine/Concrete/LibVlcAudioEngine.cs` — dual-player, prime/promote,
  event routing, re-emit on promote, `NativePlayer => _primary` invariant.
- `MediaPlayer.ViewModel/ViewModels/QueueViewModel.cs` — `PeekNextItem(bool)`.
- `MediaPlayer.ViewModel/Commands/Concrete/NextTrackCommand.cs` — use `PeekNextItem`.
- `MediaPlayer.ViewModel/ViewModels/MediaControlsViewModel.cs` — re-prime wiring,
  audio-only gate, `PlaybackSettings` import.
- `MediaPlayer.Settings/Configuration/PlaybackSettings.cs` — **new**.
- `MediaPlayer.ViewModel/ViewModels/SettingsViewModel.cs` — expose `GaplessPlayback`.
- `MediaPlayer.Common/Enumerations/MessengerMessages.cs` — `GaplessPlaybackChanged`.
- `MediaPlayer.Shell/Messenger Registrations/MessengerRegistrations.cs` — wire it.
- Settings panel XAML (the view binding `SettingsViewModel.UpdateMetadata`) — add toggle.

---

## Framework-behavior assumptions (reasoned, NOT verified — must manually verify)

1. **Prime technique:** a `Play()`→`SetPause(true)` secondary player resumes
   click-instantly with no audible artifact, at the track head (VLC may not park at
   frame 0 — may need `SeekTo(0)`).
2. **Long hold:** a primed-paused player held for the full duration of a multi-minute
   track still resumes instantly (VLC has not released/stalled its demuxer/buffer).
3. **Duration on promote:** VLC's `LengthChanged` for the primed track fires *during
   priming* — while `SelectedMediaItem` is still the previous track — so the existing
   path-stamped guard in `AudioEngine_DurationDiscovered` (line 178) drops it, and VLC
   likely will *not* re-fire it on resume. Engine therefore **explicitly re-emits**
   `DurationDiscovered` (and zero `PositionChanged`) on promote; verify the seekbar/
   duration is correct on the gaplessly-started track.

Per `CLAUDE.md` these are reasoned-from-docs, not verified; the manual checks below are
the verification gate, and the settings toggle is the escape hatch.

---

## Verification

Build/tests: `dotnet build src/MediaPlayer.sln`,
`dotnet test src/MediaPlayer.ViewModel.Test`; add unit tests for
`QueueViewModel.PeekNextItem` (next / end-no-repeat / repeat-wrap / shuffled order)
and that `NextTrackCommand` uses it (extend `MediaControlsViewModelTests`).

Manual smoke (`dotnet run --project src/MediaPlayer.Shell`):
- Two audio tracks to natural end → **no audible gap** (core acceptance).
- Manual Next early in a track and late in a track → both instant, no gap.
- Shuffle toggle, then let track end → correct (reshuffled) next plays.
- Repeat ON, last track ends → wraps to first, gapless.
- Repeat OFF, last track ends → stops cleanly, no error, no orphan primed player.
- Remove the primed/next item mid-track, then let current end → correct track plays
  (Play()-guard self-correct), no crash.
- Clear list mid-track → no crash, preload released.
- Video item as current or next → cold-load, video renders (primary invariant), no swap.
- Toggle gapless OFF mid-track → next transition has the old gap, primed player
  released, no crash; setting persists across restart (default ON on first run).
- Long (multi-minute) track → primed player still resumes instantly (assumption #2).
- Seekbar/duration correct on a gaplessly-started track (assumption #3).
- Lower volume in the last seconds of a track → next track starts at the new volume.
