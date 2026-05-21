# Visualizations Plan

This document is the agreed plan for adding audio visualizations to the app on branch `feature/visualizations`. The scope is larger than "draw bars" — it includes replacing the playback layer with LibVLC (so visualizations are sample-accurate and the audio and video paths use one engine), building a GPU-based rendering host, and authoring eight bespoke visualizers across four aesthetic families. This document reflects the design grilling that followed the initial draft; decisions that were re-opened during grilling now live in the Decisions Locked table.

---

## Context

`MediaPlayer.View/Components/HeroArea.xaml` currently uses WPF's `MediaElement` for playback. `MediaElement` is a black box: it plays audio but does not expose the raw sample stream, so genuine spectrum/waveform visualizations are impossible without changing the playback layer. Any visualization built on `MediaElement` alone has to fake reactivity or eavesdrop on system audio, both of which put a low ceiling on visual quality.

We want to do this properly. That means owning the playback pipeline end-to-end (decoder → sample tap → output device, with synchronization that matches what you hear), rendering visualizations on the GPU so the visual budget matches modern hardware, and leveraging signals this app uniquely has — chiefly the album-art-derived color palette — so visualizations feel tied to the music being played rather than generic.

**Outcome:** an optional visualization mode that replaces the HeroArea (everything else in the UI stays put), a fullscreen visualizer mode, and a foundation that hosts eight polished bespoke visualizers spanning geometric, particle, fluid/feedback, and cinematic families.

---

## Decisions Locked

| # | Decision |
|---|---|
| 1 | **Playback engine:** Replace `MediaElement` entirely with **LibVLC** (via LibVLCSharp). Single engine for both audio and video — no dual-engine split. |
| 2 | **Library license:** LibVLCSharp + the VLC native runtime (LGPL, dynamically linked, ~30MB of native binaries shipped with the app). For this personal/non-commercial project the license is fine; if distribution shape ever changes, the LGPL terms need a glance. |
| 3 | ~~**Phase 1 audio scope:** Tier 2 — parity with `MediaElement` plus gapless playback via LibVLCSharp's `MediaListPlayer`.~~ **SUPERSEDED by `docs/plans/done/ENGINE_QUEUE_REMOVAL_PLAN.md` (see Decisions 23–26 and Out of Scope).** Phase 1 shipped at `MediaElement` parity *without* gapless. The `MediaListPlayer` mechanism was deleted and is now forbidden by CLAUDE.md ("don't reintroduce `LoadPlaylist`, `PlayAt(int)`, or any list-shaped API on `IAudioEngine`"). ReplayGain / crossfade / output-device-picker deferral still stands. |
| 4 | **A/V sync:** Custom `IAudioSampleTap` with a ring buffer + sample clock that offsets visualizer frames to match LibVLC's output latency. Visuals match what you hear, not what's been decoded. `LatencyOffset` is a per-tap tunable so a visualizer can opt into a "predictive" feel with a negative offset. *(Sourcing mechanism refined by Decision 26 — a separate silent decode, not a callback on the playback player.)* |
| 5 | **Rendering:** Direct3D 11 via **Vortice.Windows**, presented into WPF through `D3DImage`. Single shared `ID3D11Device` across all visualizers; each visualizer owns its own shaders/buffers. |
| 6 | **Render thread:** Background thread executes `IVisualizer.Render()`; UI thread does the `D3DImage` lock + dirty-rect blit. Keeps the UI responsive even when a visualizer takes >16ms. |
| 7 | **Frame rate:** Tied to **display refresh rate**, vsync-presented. No battery-aware adaptive logic — the visualizer on/off toggle is the battery feature. |
| 8 | **Milkdrop / libprojectM:** **Dropped.** Eight polished bespoke visualizers instead. |
| 9 | **Visualizer set:** Eight total — **2 geometric** (`Bars`, `Oscilloscope`), **2 particle** (`BassParticleField`, `StarfieldTunnel`), **3 fluid/feedback** (`WarmFluid`, `ColdFluid`, `StrobingFluid`), **1 cinematic** (`CinematicRibbon`). |
| 10 | **Aesthetic identity:** "Milkdrop vibe but better" — modern GPU effects (compute particles, ping-pong feedback, bloom, displacement) authored against current hardware budgets, not 2003 hardware. |
| 11 | **Album-art palette integration:** `ImageSharpColorService` extended from single-dominant-color to a 3–5 color palette (k-means quantization). Palette is part of `VisualizationFrame` — every visualizer can use it. Every track gets a visual identity matching its cover. |
| 12 | **State behavior:** Two-mode state machine per visualizer — audio-reactive when `frame.AudioActive` is true; **ambient idle** (slow palette-colored procedural motion) when paused, stopped, or between tracks. |
| 13 | **UX — windowed:** `HeroArea` swaps between `AlbumArtDisplay` (current behavior) and `VisualizerArea` (new). *(As-built it is a 3-way swap — audio+off → `AlbumArtDisplay`, audio+on → `VisualizerArea`, video → `VideoView`; see Surface Changes.)* Lyrics panel, queue panel, transport bar, title bar, top toggle row — all unchanged. |
| 14 | **UX — toggle location:** New `ToggleButton` in the top toggle row of `PlayerShell.xaml`, alongside Lyrics/Queue/Settings. ~~Bound to `MainViewModel.IsVisualizerOpen`.~~ → **`PlayerShellViewModel.IsVisualizerOpen` (Decision 24 — `MainViewModel` no longer exists).** `IsEnabled` is bound to `QueueViewModel.SelectedMediaItem.IsAudio` — for video items the visualizer toggle is disabled (the video should be visible, not visualized). |
| 15 | **UX — NowPlayingMetadata:** Stays visible below the visualizer in windowed mode. |
| 16 | **UX — fullscreen entry:** Double-click on visualizer area, **or** F11. When F11 fires while the visualizer is off, it turns the visualizer **on** *and* fullscreens it (one-shot "show me the cool thing"). |
| 17 | **UX — fullscreen exit:** Double-click, F11, or ESC. **ESC exits both fullscreen and visualizer mode** (returns to `AlbumArtDisplay`), forming a clean F11/ESC toggle pair. |
| 18 | **UX — fullscreen mini-controls:** Auto-hiding overlay with play/pause, seek, prev/next, **volume slider**, track info. Fade in on mouse-move, fade out after ~3s of idle. |
| 19 | **UX — scroll wheel on visualizer:** Controls **volume** (matches universal media-player convention — YouTube, VLC, mpv). Not visualizer cycling. |
| 20 | **UX — visualizer cycling:** **Arrow keys** (Left/Right) when the visualizer area has focus, plus a name-only dropdown in the `VisualizerOverlay`. A thumbnail-grid picker is a polish follow-up. |
| 21 | **UX — fullscreen monitor:** Same monitor as the main window. Multi-monitor target selection is an explicit follow-up if asked for. |
| 22 | **Workflow:** All phases land on `feature/visualizations` — no merges to `master` until everything ships. After each phase: build + test + leave uncommitted for user local testing. Commit on the feature branch only after user signs off per phase. PR only after the final phase is approved. |
| 23 | **Gapless deferred to its own branch.** It is orthogonal to visualizations; its locked mechanism (`MediaListPlayer`) is dead and CLAUDE.md-forbidden. Phase 1 is complete *without* gapless. Likely future mechanism: an orchestrator-driven `Preload(string)` single-path hint on `IAudioEngine` (a hint, **not** a re-introduced playlist). Gets its own grilling on its own branch — see Out of Scope. |
| 24 | **Palette owned & computed by `ThemeViewModel`**, reusing the existing album-art decode in the `AutoAdjustAccent` flow (`ThemeViewModel` already owns `IColorService` and is the registered consumer of the album-art-changed signal). `IColorService` gains `GetPaletteAsync`; `GetDominantColorAsync` stays. `MainViewModel` no longer exists. |
| 25 | **Palette delivered to `IVisualizationHost` by the Shell `MessengerRegistrations` broker.** The existing `AutoAdjustAccent` handler resolves `IVisualizationHost` and pushes the palette after the accent call. `ThemeViewModel` never references `MediaPlayer.Visualization` (no Theme→Visualization coupling; no PropertyChanged-as-VM-comms). Original plan line "`[Import]` the new `IAudioEngine` … on `MainViewModel`" is **deleted** — CLAUDE.md bans importing `IAudioEngine` outside `MediaControlsViewModel`. |
| 26 | **Sample tap = a separate, silent, second libVLC decode inside `MediaPlayer.AudioEngine`**, position-mirrored off the primary player. The audible player is never touched, so silencing playback is structurally impossible. `IAudioSampleTap` is a MEF singleton sibling to `IAudioEngine`; the host `[Import]`s `IAudioSampleTap`, never `IAudioEngine`. `SyncedFrameClock` corrects tap drift (within the ≤30 ms already accepted). Replaces the original "tap the playback player via `SetAudioCallbacks`" design, which would have silenced the speakers. |

---

## Project Structure (target)

```
src/
├── MediaPlayer.AudioEngine/             ← NEW project — owns playback
│   ├── Abstract/
│   │   ├── IAudioEngine.cs              ← SHIPPED (slim, queue-unaware): Play(string)/TogglePause()/Stop()/SeekTo + Position/Duration/Volume/PlaybackState/CurrentTrackPath/NativePlayer
│   │   └── IAudioSampleTap.cs           ← NEW (Phase 2): sample broadcaster, LatencyOffset, ring buffer. MEF singleton sibling to IAudioEngine; host imports THIS, never IAudioEngine
│   ├── Concrete/
│   │   ├── LibVlcAudioEngine.cs         ← SHIPPED: LibVLCSharp-backed single-file decoder (no MediaListPlayer, no gapless — see ENGINE_QUEUE_REMOVAL)
│   │   ├── LibVlcSampleTap.cs           ← NEW (Phase 2): a SECOND, silent libVLC decode of the same file, position-mirrored off the primary; primary player untouched (Decision 26)
│   │   └── SyncedFrameClock.cs          ← NEW (Phase 2): tracks libVLC output latency; offsets ring-buffer reads to playback and corrects secondary-decoder drift
│   └── Events/
│       ├── PlaybackPositionChangedEventArgs.cs
│       ├── PlaybackStateChangedEventArgs.cs
│       └── PlaybackEndedEventArgs.cs
│
├── MediaPlayer.Visualization/           ← NEW project — owns visualizer host & built-ins
│   ├── Abstract/
│   │   ├── IVisualizer.cs               ← Name, Initialize, Render(frame), Resize, Dispose
│   │   ├── IVisualizationHost.cs        ← current visualizer, D3D device, frame pump, target swap
│   │   └── IVisualizationFrameSource.cs ← composes audio + palette + state into VisualizationFrame
│   ├── Concrete/
│   │   ├── D3D11VisualizationHost.cs    ← Vortice device, background render thread, D3DImage blit
│   │   ├── VisualizationFrameSource.cs  ← consumes IAudioSampleTap; receives palette from ViewModel
│   │   └── Visualizers/
│   │       ├── BarsVisualizer.cs                ← geometric: FFT bars (default)
│   │       ├── OscilloscopeVisualizer.cs        ← geometric: waveform line
│   │       ├── BassParticleFieldVisualizer.cs   ← particle: bass-reactive points (compute shader)
│   │       ├── StarfieldTunnelVisualizer.cs     ← particle: tunneling starfield
│   │       ├── WarmFluidVisualizer.cs           ← fluid/feedback: warm/organic
│   │       ├── ColdFluidVisualizer.cs           ← fluid/feedback: cold/clinical
│   │       ├── StrobingFluidVisualizer.cs       ← fluid/feedback: aggressive/strobing
│   │       └── CinematicRibbonVisualizer.cs     ← cinematic: ribbons + displacement
│   └── Frame/
│       └── VisualizationFrame.cs        ← FFT[], waveform[], peak, beat, palette[], time, deltaTime, AudioActive
│
└── MediaPlayer.View/
    └── Components/
        ├── HeroArea.xaml                  ← MODIFIED: AlbumArt ↔ Visualizer swap on IsVisualizerOpen; video items show LibVLC VideoView
        ├── VisualizerArea.xaml/.cs        ← NEW: hosts D3DImage; handles double-click, scroll-wheel volume, arrow-key cycling, focus
        ├── VisualizerOverlay.xaml         ← NEW: visualizer-name dropdown, fullscreen button
        ├── FullscreenVisualizerWindow.xaml/.cs    ← NEW: borderless topmost window for fullscreen
        └── FullscreenControlsOverlay.xaml ← NEW: auto-hiding mini transport (play/pause/seek/prev/next/volume + track info)
```

### Project references

- `MediaPlayer.AudioEngine` → `MediaPlayer.Common`, `Generic` (DI, messenger). LibVLCSharp + `VideoLAN.LibVLC.Windows` NuGets. **SHIPPED.** `IAudioSampleTap` is added here in Phase 2 as a sibling to `IAudioEngine`.
- `MediaPlayer.Visualization` → `MediaPlayer.AudioEngine` (for `IAudioSampleTap`), `MediaPlayer.Common`, `Generic`. Vortice.Windows NuGet. **Phase 2.**
- `MediaPlayer.ViewModel` → references `MediaPlayer.AudioEngine` only (already, for the engine). It does **not** reference `MediaPlayer.Visualization` — palette delivery is Shell-brokered (Decision 25), and `PlayerShellViewModel.IsVisualizerOpen`/`IsVisualizerFullscreen` are plain `bool`s with no visualization types.
- `MediaPlayer.View` → adds reference to `MediaPlayer.Visualization` (for the `D3DImage` host control). LibVLCSharp.WPF (`VideoView` for video items) is **already wired** (`HeroArea.xaml`).
- `MediaPlayer.Shell` → resolves `IVisualizationHost` from the MEF container in `MessengerRegistrations` to broker the palette push (Decision 25).

---

## Surface Changes Outside the New Projects

> **Note:** the entire `MediaElement` → `IAudioEngine` surface (the old `MediaControlsViewModel` MediaElement-driven properties, `MediaOpenedCommand`, the deleted converters/extensions) **shipped in Phase 1**, then was reshaped by `ENGINE_QUEUE_REMOVAL`. See the "Phase 1 — Complete (as-built)" section for the as-built state. The bullets below are the **remaining Phase 2+ surface**, mapped to the current 6-VM layout (no `MainViewModel`).

### `MediaPlayer.ViewModel`

- **`ThemeViewModel`** (Decision 24): owns the palette. Extend `IColorService` with `GetPaletteAsync(byte[] imageBytes, int count = 5)` (ordered `Color` list, k-means/octree via ImageSharp); `GetDominantColorAsync` stays (auto-accent unchanged). `ThemeViewModel` computes the palette in/alongside the existing `AutoAdjustAccentAsync(byte[])` flow — **one** album-art decode, two outputs (accent + palette) — and exposes it for the Shell broker to read. `ThemeViewModel` does **not** reference `MediaPlayer.Visualization`.
  - `IColorService` namespace is `MediaPlayer.Settings.Services.Abstract` (file under `MediaPlayer.ViewModel/Services/`), exported as `ServiceNames.ImageSharpColorService` — *not* `MediaPlayer.ViewModel` as the original plan stated.
- **`PlayerShellViewModel`** (Decision 24/25): add `IsVisualizerOpen` (`bool`) and `IsVisualizerFullscreen` (`bool`) — plain notify-properties mirroring the existing `IsLyricsOpen`/`IsQueueOpen`/`IsSettingsOpen` (`PlayerShellViewModel.cs:16-44`). `[Import]` `ToggleVisualizerCommand` and `ToggleVisualizerFullscreenCommand` (new `CommandNames` constants), mirroring the existing `[Import] ToggleLyricsCommand` pattern.
- **`Commands/Concrete/EscapeCommand.cs`**: already operates on `PlayerShellViewModel` (`EscapeCommand.cs:17-38`). Prepend two guard branches *ahead of* the existing settings/lyrics logic — (1) if `IsVisualizerFullscreen` → exit fullscreen; (2) else if `IsVisualizerOpen` → exit visualizer mode — and widen `CanExecute` to include those states. Order: fullscreen → visualizer → settings → lyrics.
- **New toggle commands** follow the established command convention: `[Import]` lives on `PlayerShellViewModel` (the VM whose state they mutate); `CommandParameter` binds `PlayerShellViewModel`; `Execute` casts the parameter (no MEF `[Import]` of a VM inside the command).
- **DELETED from the original plan:** the line "`[Import]` the new `IAudioEngine` … on `MainViewModel` / `MediaControlsViewModel`." CLAUDE.md forbids `[Import] IAudioEngine` anywhere except `MediaControlsViewModel`. The visualization path gets audio via `IAudioSampleTap` (Decision 26), not an engine import.

### `MediaPlayer.View`

- `HeroArea.xaml`: **already** strips `MediaElement` and shows `AlbumArtDisplay` (audio) / `vlc:VideoView` (video, bound to `MediaControlsViewModel.AudioEngine.NativePlayer`) — a 2-way swap on `QueueViewModel.SelectedMediaItem.IsAudio`/`IsVideo` (`HeroArea.xaml:19-25`). Phase 2 **adds a third state**: audio + `IsVisualizerOpen` → `VisualizerArea`; audio + visualizer-off → `AlbumArtDisplay` (current); video → `VideoView` (unchanged). `NowPlayingMetadata` stays in row 1.
- `PlayerShell.xaml`: add a **fourth** `ToggleButton` to the existing top StackPanel (`PlayerShell.xaml:20-53`, currently Lyrics/Queue/Settings), `IsChecked` two-way to `IsVisualizerOpen`, `IsEnabled` to `QueueViewModel.SelectedMediaItem.IsAudio`. `SymbolIcon` e.g. `EqualizerArrowsClockwise24`.
- `ViewMediaPlayer.xaml`: add `<KeyBinding Key="F11" Command="{Binding ToggleVisualizerFullscreenCommand}" CommandParameter="{Binding}" />` to the existing `FluentWindow.InputBindings` block (`ViewMediaPlayer.xaml:26-37`). The ESC binding is at `:36` (`CommandParameter="{Binding}"` = the root `PlayerShellViewModel`); it stays — behavior change is inside `EscapeCommand`.
- `MediaElementOpenedMultiValueConverter` / `MediaElementExtension`: **already deleted** in Phase 1 (confirmed in git status).

### `MediaPlayer.Shell`

- `App.xaml.cs` / `MEF.ComposeAll`: ensure `MediaPlayer.Visualization` is picked up (`MediaPlayer.AudioEngine` already is). LibVLC `Core.Initialize()` is **already** wired at startup.
- `MessengerRegistrations.cs` (Decision 25): the existing `AutoAdjustAccent` handler (`MessengerRegistrations.cs:55-69`) — which already resolves `ThemeViewModel` and calls `AutoAdjustAccentAsync` — also resolves `IVisualizationHost` from the container and calls `SetPalette(...)` after the accent call. Per the CLAUDE.md "add the subscription to the existing method" rule, this goes **in the existing `AutoAdjustAccent` method**, not a new one. No new messenger message.

### `MediaPlayer.ViewModel.Test`

- The `MediaElement` → `Mock<IAudioEngine>` test retarget **already happened** in Phase 1. `MainViewModelTests.cs` is **deleted** (the VM no longer exists); engine-interaction tests live in `MediaControlsViewModelTests.cs`. Phase 2 adds **palette tests on `ThemeViewModel` / `IColorService`** (`GetPaletteAsync` ordering/count, reuse of the single decode). The tap/host/D3D path is largely manual-verified (native + GPU) — see Risks.

---

## Architecture Detail

### Audio engine (`IAudioEngine`) — SHIPPED, reshaped by `ENGINE_QUEUE_REMOVAL`

The original playlist-engine design below is **dead**. The shipped engine is a slim, queue-unaware single-file decoder. Authoritative spec: `docs/plans/done/ENGINE_QUEUE_REMOVAL_PLAN.md`. Current shape (`src/MediaPlayer.AudioEngine/Abstract/IAudioEngine.cs`):

```
IAudioEngine                                 ← single-file decoder; the QUEUE is QueueViewModel.MediaItems
  Play(string path)                          ← load + start one file
  TogglePause()                              ← state-dependent pause/resume/restart
  Stop()
  SeekTo(TimeSpan position)
  Position / Duration { get; }
  Volume { get; set; }                       ← 0.0..1.0
  PlaybackState { get; }                     ← Playing / Paused / Stopped   (NO Ended)
  CurrentTrackPath { get; }
  NativePlayer { get; }                      ← VlcMediaPlayer; load-bearing for HeroArea VideoView
  event PositionChanged / StateChanged / DurationDiscovered / TrackEnded
```

Queue navigation (next/prev/repeat-wrap/end-of-queue-stop) lives in commands; `MediaControlsViewModel`'s `TrackEnded` handler delegates to `NextTrackCommand` with a `Stop()` fallback. **Do not** reintroduce `LoadPlaylist`/`PlayAt(int)`/`NextTrack`/`PreviousTrack`/`PlaybackState.Ended` — CLAUDE.md-forbidden. `LibVlcAudioEngine` is a MEF singleton wrapping one `LibVLC` + one `MediaPlayer`; libVLC fires events on its own threads, the engine marshals onto the WPF dispatcher before re-raising.

### Sample tap & sync clock (`IAudioSampleTap` + `SyncedFrameClock`) — Phase 2, Decision 26

```
IAudioSampleTap                              ← MEF singleton in MediaPlayer.AudioEngine, sibling to IAudioEngine
  TimeSpan LatencyOffset { get; set; }       ← default = libVLC output latency; negative = predictive
  event FrameAvailable                       ← float[1024] FFT + float[1024] waveform + sync-corrected playback time
  bool AudioActive { get; }                  ← true iff audio is playing AND samples are flowing
```

**The tap does NOT touch the audible player.** libVLC's `SetAudioCallbacks` installs the `amem` *output module* — it is output-*replacing* (like its mirror `SetVideoCallbacks`), so tapping the playback player would silence the speakers. Instead, `LibVlcSampleTap` runs a **second, independent libVLC decode** of the same file with `amem` and no device output, **position-mirrored** off the primary player (the engine drives it internally on `Play`/`SeekTo`/`Stop` — same assembly, no VM imports). The primary playback path is byte-for-byte unchanged, so silencing playback is structurally impossible.

`LibVlcSampleTap` keeps a 500 ms ring buffer of samples from the secondary decode. `SyncedFrameClock` consumes libVLC's reported output latency + the engine's playback position to (a) pick the ring-buffer slice that matches "what you hear right now" and (b) correct secondary-decoder drift; `FrameAvailable` is raised at display-refresh cadence with FFT/waveform from that slice. 500 ms comfortably exceeds typical 50–150 ms latency plus drift headroom. *(libVLC `amem` output-replacement behavior is reasoned from libVLC architecture + the symmetric `SetVideoCallbacks` behavior — see Risks; the Phase 2 spike verifies it empirically.)*

### Visualization host (`IVisualizationHost`)

Owns:
- A single `ID3D11Device` shared across all visualizers
- A render-target `D3D11Texture2D` exposed to WPF via `D3DImage` shared-surface handle
- The currently active `IVisualizer`
- A background render thread that pulls `VisualizationFrame` and calls `Render(frame)`, then signals the UI thread to lock the `D3DImage` and blit

```
IVisualizationHost
  RegisterTarget(D3DImage image, int width, int height)
  UnregisterTarget(D3DImage image)
  CurrentVisualizer { get; set; }
  AvailableVisualizers { get; }              ← IEnumerable<IVisualizer> from MEF [ImportMany]
  SetPalette(IReadOnlyList<Color> palette)   ← pushed by the Shell MessengerRegistrations broker on album-art change (Decision 25)
  NextVisualizer() / PreviousVisualizer()
```

MEF lifecycle: each `IVisualizer` is a singleton, instantiated once. D3D resources are lazily allocated in `Initialize()` on first selection — keeps memory cost low for the seven visualizers that aren't currently selected. `Dispose()` is reserved for app-shutdown and visualizer crash recovery.

### Visualizer interface (`IVisualizer`)

```
IVisualizer
  string Name { get; }                       ← e.g. "Bars", "Warm Fluid"
  void Initialize(ID3D11Device device, ID3D11DeviceContext context)
  void Resize(int width, int height)
  void Render(VisualizationFrame frame)      ← frame.AudioActive determines reactive vs ambient mode
  void Dispose()
```

### Visualization frame

```
VisualizationFrame
  float[] FftMagnitudes        ← 1024 bins, log-scaled
  float[] Waveform             ← 1024 raw samples
  float Peak / float BassEnergy / float BeatStrength
  IReadOnlyList<Color> Palette ← 3–5 swatches from album art
  TimeSpan PlaybackTime
  float DeltaTime              ← seconds since last frame
  bool AudioActive             ← reactive when true, ambient when false
```

### Crash recovery

A `try/catch` around the per-frame `IVisualizer.Render` in the host's render loop catches any visualizer-side exception, disposes the failing visualizer, drops it from `AvailableVisualizers`, raises a one-time toast through the messenger, and falls the host back to `BarsVisualizer` (the simplest, always-available baseline). LibVLC native-load failures at startup put the engine in a "no playback available" state and the app shows an error toast — the rest of the UI remains usable for inspection.

### UX flow — windowed → fullscreen

Windowed: `VisualizerArea` (UserControl) sits inside `HeroArea`. Hosts an `<Image Source="{Binding D3DImage}"/>`. `IVisualizationHost.RegisterTarget` is called on Loaded.

Fullscreen: when `IsVisualizerFullscreen` flips true, a `FullscreenVisualizerWindow` (borderless, topmost, `WindowState = Maximized`) opens on the same monitor as the main window. The host's render target is re-pointed to the fullscreen window's `D3DImage`; the windowed `VisualizerArea` shows a "Visualizer is in fullscreen" placeholder. On exit, the fullscreen window closes and the host re-points back. Only one `D3DImage` active at a time. `FullscreenControlsOverlay` (auto-hiding) lives on the fullscreen window and binds to the same `MediaControlsViewModel`. Mouse-move resets a 3-second visibility timer; idle hides with a fade.

---

## Phased Implementation

Each phase ends in a runnable, releasable state. Per the workflow: build + test + leave uncommitted for user testing → commit on signoff → next phase. No PR until the final phase signs off.

### Phase 1 — LibVLC engine replacement — ✅ COMPLETE (as-built)

Goal (original): app works exactly as today (plus gapless), `MediaElement` gone.
**As shipped: `MediaElement` is gone and at-parity; gapless was descoped (Decision 23).**

What shipped:
1. ✅ `MediaPlayer.AudioEngine` project created; LibVLCSharp + `VideoLAN.LibVLC.Windows` NuGets added; `Core.Initialize()` wired at Shell startup.
2. ⚠️ `IAudioEngine` / `LibVlcAudioEngine` implemented — **then reshaped by `docs/plans/done/ENGINE_QUEUE_REMOVAL_PLAN.md`**: the `MediaListPlayer`/playlist engine was replaced with a single-file decoder (`Play(string)`/`TogglePause()`/`Stop()`/`SeekTo`; events `PositionChanged`/`StateChanged`/`DurationDiscovered`/`TrackEnded`; `PlaybackState` = Playing/Paused/Stopped). **No gapless** — its `MediaListPlayer` mechanism was deleted and is CLAUDE.md-forbidden.
3. ✅ `MediaControlsViewModel` wired to `IAudioEngine`; queue navigation lives in `NextTrackCommand`/`PreviousTrackCommand` + the `TrackEnded`→`NextTrackCommand` delegation (per ENGINE_QUEUE_REMOVAL).
4. ✅ `HeroArea.xaml` strips `MediaElement`; audio → `AlbumArtDisplay`, video → `vlc:VideoView` bound to `MediaControlsViewModel.AudioEngine.NativePlayer` (`HeroArea.xaml:19-25`).
5. ✅ `MediaElementOpenedMultiValueConverter`, `MediaElementExtension`, `MediaOpenedConverterModel`, `MediaOpenedCommand` deleted (confirmed in git status).
6. ✅ Tests retargeted to `Mock<IAudioEngine>`; `MainViewModelTests.cs` deleted with the VM split; engine tests live in `MediaControlsViewModelTests.cs`.

**Deviations from the original plan (do not treat as TODO):** gapless removed from scope (now Out of Scope, own branch); engine surface is the slim `ENGINE_QUEUE_REMOVAL` shape, not the playlist shape; `MainViewModel` was split into 6 VMs by a separate refactor.

**Residual Phase 1 verification still owed before final PR:** behavior parity across MP3/FLAC/WAV; video plays via the LibVLCSharp.WPF `VideoView` (historical WPF airspace risk — verify the modern shared-surface path behaves on the target machine before final sign-off); regression-check lyrics/queue/theme/settings/drag-drop. The original "gapless check" line is **removed** (out of scope).

### Phase 2 — Visualization infrastructure + first two visualizers + fullscreen

Goal: complete framework. Toggle the visualizer, see bars/oscilloscope dance to the music, fullscreen works.

0. **GATING SPIKE (do this first — the rest of Phase 2 builds on it).** Prove the Decision-26 tap: (a) empirically confirm libVLC `amem`/`SetAudioCallbacks` is output-replacing (a tap on the playback player silences it); (b) stand up a throwaway second silent decode of a playing file, position-mirrored off the primary, and confirm correct FFT/waveform from a known test tone **while the primary track still plays audibly**. If (b) fails, stop and re-grill the tap mechanism before writing any visualizer. *(Framework-behavior gate per CLAUDE.md — reasoned, not yet verified.)*
1. Create `MediaPlayer.Visualization` project. Add Vortice.Windows NuGet. Add `IAudioSampleTap` to `MediaPlayer.AudioEngine` as a MEF singleton sibling to `IAudioEngine`.
2. Extend `IColorService` (namespace `MediaPlayer.Settings.Services.Abstract`) with `GetPaletteAsync(byte[], int=5)`. `ThemeViewModel` computes the palette in/alongside `AutoAdjustAccentAsync` (one decode, accent + palette) and exposes it. `GetDominantColorAsync` unchanged.
3. Implement `LibVlcSampleTap` (second silent decode, position-mirrored, primary untouched) + `SyncedFrameClock`. Verify FFT/waveform against a known test tone *and* that primary audio still plays.
4. Implement `D3D11VisualizationHost` with `D3DImage` shared-surface interop + background render thread. Verify a solid-color visualizer renders end-to-end.
5. Implement `BarsVisualizer` and `OscilloscopeVisualizer`. Both consume the palette and respect `AudioActive` (ambient = slow palette-colored sweep/pulse).
6. `PlayerShellViewModel`: add `IsVisualizerOpen`/`IsVisualizerFullscreen` (`bool`, mirroring `IsLyricsOpen`); `[Import]` the two new toggle commands (new `CommandNames` constants). Create `VisualizerArea` + `VisualizerOverlay` (name-only dropdown, fullscreen button). Add the 4th `ToggleButton` to `PlayerShell.xaml`'s top StackPanel (`IsChecked`→`IsVisualizerOpen`, `IsEnabled`→`QueueViewModel.SelectedMediaItem.IsAudio`).
7. `HeroArea.xaml`: extend the current 2-way (`IsAudio`/`IsVideo`) swap to 3-way — audio + `IsVisualizerOpen` → `VisualizerArea`; audio + off → `AlbumArtDisplay`; video → `VideoView` (unchanged). `NowPlayingMetadata` stays in row 1.
8. Wire the Shell `MessengerRegistrations.AutoAdjustAccent` handler to also resolve `IVisualizationHost` and `SetPalette(...)` after the accent call (Decision 25 — same existing method, no new message).
9. `FullscreenVisualizerWindow` + `FullscreenControlsOverlay` (auto-hiding mini transport w/ volume). Add the F11 `KeyBinding` (`CommandParameter="{Binding}"`) to `ViewMediaPlayer.xaml`'s `InputBindings`; extend `EscapeCommand` (fullscreen → visualizer → settings → lyrics). Double-click → fullscreen; scroll-wheel-on-visualizer → volume; arrow keys → cycle (bars ↔ oscilloscope at this stage).
10. **Verification:** toggle swaps the hero; bars/oscilloscope react in sync (≤16 ms perceived); both use the track palette; pause → ambient; track change → brief ambient → reactive; scroll = volume; arrows cycle; F11 off→on+fullscreen; ESC exits fullscreen *and* visualizer; double-click toggles fullscreen; mini controls fade; **all Phase 1 behavior unchanged and primary audio audible throughout**.

### Phase 3 — Particle visualizers

1. `BassParticleFieldVisualizer` — compute-shader-driven particle field reacting to bass energy + beat strength.
2. `StarfieldTunnelVisualizer` — tunneling starfield with palette-driven star tinting; speed reacts to tempo / overall energy.
3. Both implement reactive and ambient modes.
4. **Verification:** both run at display refresh rate. Visualizer cycling now spans 4 items. Memory cost reasonable (lazy D3D resource init means inactive visualizers are cheap).

### Phase 4 — Fluid/feedback visualizers

1. `WarmFluidVisualizer` — ping-pong-buffer feedback with warm palette bias and organic motion.
2. `ColdFluidVisualizer` — same engine, cold/clinical palette bias, sharper / more geometric warps.
3. `StrobingFluidVisualizer` — aggressive feedback with beat-locked strobing / inversion (with a `BeatStrength` threshold to avoid epileptic-trigger territory by default).
4. **Verification:** all three run smoothly. Palette feeds drive the family identity (same engine produces visibly distinct moods per visualizer). Cycling spans 7 items.

### Phase 5 — Cinematic visualizer + picker polish

1. `CinematicRibbonVisualizer` — ribbon geometry with audio-reactive displacement, bloom, palette-driven lighting. The "wow shot" visualizer.
2. `VisualizerOverlay` picker polish — keep the name-dropdown but improve animation and add a "next" / "previous" button pair for users who prefer clicks.
3. **Verification:** all 8 visualizers cycle smoothly; cinematic visualizer hits display refresh on the target hardware. End-to-end manual run-through against the full Decisions Locked checklist.

---

## Out of Scope (Explicitly Deferred)

- **Custom DSP / EQ UI.** LibVLC supports it; this work doesn't add UI for it.
- **ASIO / WASAPI exclusive mode UI.** LibVLC supports specific output modules; UI for choosing them is a separate feature.
- **Visualizer preset editor / sliders.** Built-in visualizers are intentionally not parameter-tunable in v1.
- **SMTC integration changes.** Continues as today.
- **Mini-player / picture-in-picture.** Separate feature.
- **Visualizing video's audio track.** Toggle disabled for video items. Reconsider if asked for.
- **Lyric-aware visualizers.** A "concert-poster" style visualizer that hero-styles the current synced lyric line, and a subtle ambient-lyric overlay on the cinematic visualizer, are both desired follow-ups. Blocked on first adding synced-lyric (LRC) infrastructure — the current `LyricsOvhApi` returns plain text only. Should be its own branch: investigate LRC sources (LRClib / NetEase / Musixmatch) and licensing, parse LRC format, expose a "current line + progress" signal from `MediaControlsViewModel`, then add lyric-aware visualizers in a follow-up to this work.
- **Audiophile "Tier 3" features.** Each has its own UX question and deserves its own grilling/branch — bundling into the visualizations work would balloon Phase 1 risk:
  - **Gapless playback** — *(was Decision 3, locked; descoped per Decision 23.)* Zero silence between continuously-recorded tracks. Original `MediaListPlayer` mechanism was deleted by `ENGINE_QUEUE_REMOVAL` and is now CLAUDE.md-forbidden (no list-shaped engine API). Likely future mechanism: an orchestrator-driven `Preload(string)` single-path hint on `IAudioEngine` — the orchestrator (which owns the queue) tells the engine the next path so it can pre-open/pre-buffer a second media and swap on `EndReached` with no decode gap. This is a *hint*, not a re-introduced playlist, so it stays CLAUDE.md-legal. Orthogonal to visualizations; its own branch + own grilling (preload lifecycle on manual skip vs natural end, seek-during-preload, video items, error fallback).
  - **ReplayGain** — auto-loudness-leveling between tracks. UX question: track-based vs album-based normalization, and whether to fall back when tags are missing. LibVLC supports this natively once enabled.
  - **Configurable crossfade** — fade out the outgoing track while fading in the incoming. UX question: per-track or global setting; default duration; behavior on manual skip vs natural end-of-track.
  - **Output device picker** — let user route between Headphones / Speakers / external DAC at runtime. UX question: where it lives (transport bar quick-toggle vs settings only), and whether to remember per-device-class preferences. LibVLC exposes `AudioOutputDevice` enumeration; the UI side is the work.
- **Multi-monitor fullscreen target selection.** Fullscreen lands on the same monitor as the main window. A settings option to pick a target monitor is a follow-up if asked for.
- **Visualizer thumbnail-grid picker.** Name-only dropdown for v1; thumbnail grid is polish.

## Future Opportunities Unlocked

- **Palette-driven UI beyond visualizations.** This plan extends `ImageSharpColorService` from extracting a single dominant color (the current auto-accent feature) to a 3–5 color palette (for visualizers). Once that palette pipeline exists, it opens UI opportunities throughout the rest of the app: secondary-accent surfaces (cards, hover states, panel borders that pull from non-primary palette swatches), gradient backdrops seeded by the full palette instead of a single solid color, per-track-styled chrome that's richer than a single accent shift, palette-driven empty-state art, etc. Worth a separate design pass after this work lands to identify the highest-value UI applications. The infrastructure cost is already paid by this plan; the follow-up is pure design + binding work.

---

## Risks Accepted

| Risk | Mitigation |
|---|---|
| **LibVLC license (LGPL)** is fine for this personal/non-commercial project. If distribution shape ever changes, LGPL requires the libVLC binaries remain user-replaceable (which they already are as separate DLLs). | Review license terms before any change in distribution intent. |
| **Audio tap silences playback** — libVLC `amem`/`SetAudioCallbacks` is an output *replacement* (mirror of `SetVideoCallbacks`); tapping the playback player would mute the speakers. | **Structurally mitigated (Decision 26):** the tap is a separate silent second decode; the audible player is never touched, so silencing is impossible by construction. *Residual:* libVLC `amem` replacement behavior is reasoned-from-architecture, **not verified** — Phase 2 step 0 is a gating spike that empirically confirms it and confirms the secondary-decode tap yields correct FFT while primary audio plays. Per CLAUDE.md framework-behavior rule. |
| **Tap residuals (Option C):** second audio decode costs CPU; the secondary decoder can drift from the primary. | Audio decode is negligible vs. the D3D visualizers. `SyncedFrameClock` re-aligns the secondary decoder against the engine's reported position; worst case folds into the ≤30 ms A/V budget already accepted below. |
| **LibVLCSharp.WPF airspace issues** with the `VideoView` for video items. | Use the modern shared-surface path; verify on the target machine before final PR. *(The original "fall back to `MediaElement` for video" safety net is **gone** — `MediaElement` was deleted in Phase 1; there is no dual-engine fallback. If airspace fails, the fix is LibVLCSharp.WPF-side, not a `MediaElement` reintroduction.)* |
| **LibVLC output latency** isn't perfectly stable (depends on output device buffer size and the audio module in use). | `SyncedFrameClock` re-reads latency periodically and re-tunes the offset. Worst case: ~30ms perceived A/V drift, still better than uncorrected. |
| **`D3DImage` perf** — shared-surface copy isn't free. | Vortice + D3D11 keyed-mutex shared surfaces is the standard pattern; measure in Phase 2. Background render thread keeps UI thread unaffected. |
| **Visualizer shader crash** could break the host. | `try/catch` around `Render` per frame; failing visualizer is disposed and removed from `AvailableVisualizers`, host falls back to `BarsVisualizer`, toast notifies user. |
| **Native binary size** — ~30MB of VLC DLLs ships with the app. | Acceptable for a personal-use desktop app; revisit only if distribution shape changes. |
| **Single-instance + MEF singletons** mean engine and host are process-wide. | Per CLAUDE.md, this is by design — exactly one instance exists. State held on commands/services is safe. |
| ~~**Test surface churn** — every `MediaElement`-touching test needs to retarget to `IAudioEngine`.~~ | **Resolved in Phase 1** — retarget done; `MainViewModelTests.cs` deleted with the VM split; engine tests in `MediaControlsViewModelTests.cs`. Phase 2 adds palette tests on `ThemeViewModel`/`IColorService`; tap/host/D3D path is manual-verified (native + GPU). |

---

## Verification (overall)

1. `dotnet build src/MediaPlayer.sln` clean across all phases.
2. `dotnet test src/MediaPlayer.ViewModel.Test` green — `IAudioEngine` tests shipped in Phase 1; Phase 2 adds palette-extension tests on `ThemeViewModel`/`IColorService`.
3. `dotnet run --project src/MediaPlayer.Shell` manual run-through against this checklist at the end of each phase, and a final full pass before PR:
   - All previously-working flows behave identically to `master` (open files, play, pause, seek, volume, queue, lyrics, settings, theme, drag-drop).
   - Primary audio remains audible at all times with the visualizer on (the tap never touches the playback player).
   - Visualizer toggle in top row; disabled for video; enabled for audio.
   - Toggle on: HeroArea shows visualizer; queue/lyrics/transport/title bar all unchanged and functional.
   - Visualizer reacts to audio in correct sync.
   - Album-art palette visibly drives visualizer colors per track.
   - Pause / stop / track-gap → ambient idle; resume → reactive.
   - Scroll wheel on visualizer changes volume.
   - Arrow keys cycle visualizers; picker dropdown also works.
   - F11 with visualizer off → turns on + fullscreens. F11 with visualizer on → fullscreens.
   - ESC exits fullscreen *and* visualizer mode.
   - Double-click toggles fullscreen.
   - Mini controls fade in on mouse-move, fade out after ~3s idle.
   - All 8 visualizers cycle, render at display refresh, and look like the aesthetic family they belong to.
4. Per-phase verification lives in each phase section above.
