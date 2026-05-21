# Post-Modernization Review

Companion to [UI_MODERNIZATION_PLAN.md](./UI_MODERNIZATION_PLAN.md). Captures the decisions made after the `feat/wpf-ui-modernization` work landed in the views, before the PR was merged.

---

## ViewModel refactor assessment

After the views were decomposed into ~10 focused UserControls (`PlayerShell`, `HeroArea`, `AlbumArtDisplay`, `NowPlayingMetadata`, `EmptyState`, `LyricsPanel`, `QueuePanel`, `SettingsFlyout`, `TransportBar`, `SeekStrip`, `VolumeControl`), we asked whether the ViewModels should follow suit.

ViewModels at the time of review:

| ViewModel | Lines | Concerns |
|---|---|---|
| `MainViewModel` | 268 | Playlist, navigation helpers, shell open/close, metadata I/O orchestration, busy delegation |
| `MediaControlsViewModel` | 152 | Transport state, volume, seekbar position |
| `BusyViewModel` | 62 | Async overlay state machine |
| `SettingsViewModel` | 42 | Facade over `MetadataSettings` + `ThemeViewModel` |
| `ThemeViewModel` | 100 | Theme settings + accent extraction from album art |

### Decision: do not split, refactor, or rename the ViewModels.

Reasoning:

1. **Views split on visual cohesion; ViewModels are organized on state cohesion.** The view split was about extracting reusable presenters. Forcing a 1:1 view-to-VM mapping would produce anemic VMs that just forward properties.
2. **Each splitting candidate fails on examination.** Splitting `MediaControlsViewModel` would fragment the implicit "currently-playing media session" identity into three coordinating VMs. Splitting `MainViewModel` would either require a new `DataContext={Binding ...}` pattern in nested views (excluded by the no-new-patterns constraint) or just move bindings through a holder VM with no benefit. A `QueueViewModel` would have to aggregate state from Main + Busy + MediaControls — relocating coupling rather than reducing it.
3. **Commit `46d49de` was a deliberate consolidation in the opposite direction.** It moved `IsLyricsOpen`/`IsQueueOpen`/`IsSettingsOpen` *into* `MainViewModel` so keyboard shortcuts could drive them via `KeyBindings`. Re-extracting shell state would reverse that.
4. **The right sub-VM boundary already exists** at `SettingsFlyout` ↔ `SettingsViewModel`, where the sub-domain (independent persistence, independent sub-tree of bindings) justifies it.

### What was rejected and why

| Candidate change | Rejected because |
|---|---|
| Split `MediaControlsViewModel` 3-ways (transport/volume/seekbar) | Three sub-states are facets of one playback session identity |
| Extract `QueueViewModel` | Would aggregate from Main + Busy + MediaControls — relocates coupling |
| Extract `ShellViewModel` for panel toggles | Reverses `46d49de`; needs a new DataContext-binding pattern |
| Extract `MetadataOrchestratorViewModel` | Operates on `MediaItems`, which lives in `MainViewModel` |
| Rename `MainViewModel` → `ShellViewModel` / `AppViewModel` | Pure churn |
| Rename `MediaControlsViewModel` → `PlaybackViewModel` | Marginally more accurate but cosmetic |

---

## Pre-merge PR review

After the VM assessment, we reviewed the full `feat/wpf-ui-modernization` branch (~23 commits, ~1.5k +/− 1k lines) for things worth catching before merge.

### Addressed in this PR

#### A. `UI_MODERNIZATION_PLAN.md` moved into `docs/`

The plan was originally added at the repo root in commit `afaadfa`. Moved to `docs/` to sit alongside `DWM_BACKDROP_PLAN.md` and `UPGRADE_PLAN.md`.

#### C. `IPartImportsSatisfiedNotification` instead of `MEF.Container?.SatisfyImportsOnce(this)`

Two ctor-time workarounds replaced with the MEF-native pattern:
- `MediaPlayer.ViewModel/ViewModels/MediaControlsViewModel.cs` — moved the `SeekbarPreviewMouseUpCommand.ChangeMediaPosition` event subscription out of the constructor and into `OnImportsSatisfied()`. The class now implements `IPartImportsSatisfiedNotification`.
- `MediaPlayer.View/Views/ViewMediaPlayer.xaml.cs` — the manual call was redundant, since MEF satisfies property imports automatically when composing a part via `[Export]` (the `View` is obtained through `container.GetExports<ViewMediaPlayer>()` in `MessengerRegistrations.OpenMainWindow`). The `ViewModel` setter still wires up `DataContext` when MEF assigns it. Call removed; no replacement needed.

#### D. Tick handler leak in `MediaOpenedCommand`

Pre-existing bug: `vm.PositionTracker.Tick += (sender, args) => TrackMediaPosition(model);` had no matching `-=`. Each track change accumulated a captured `model` (holding `MediaElement` and `MainViewModel`) plus an additional handler — leading to N redundant `TrackMediaPosition` calls per Tick after N track changes.

Fixed by tracking the previous handler in a private field on the command, removing it before each new subscription. This pattern relies on the singleton-ness of MEF-exported commands and the fact that the app is single-instance only (see "Single Instance & Named Pipes" in `CLAUDE.md`).

#### E. `UpdateMetadataTokenSources` encapsulation

Was `public List<CancellationTokenSource> UpdateMetadataTokenSources` despite being read/written only from inside `MainViewModel`. Changed to `private readonly List<CancellationTokenSource> _updateMetadataTokenSources`, matching the `readonly` style of `PositionTracker` and the standard `_underscore` convention for private fields.

### Deferred

#### B. Tests for `MainViewModel.RemoveMediaItem`

The method has three meaningfully distinct branches (not-currently-playing / currently-playing-and-only-item / currently-playing-with-others) and was reworked twice during the modernization. Adding tests is desirable but there's more to unpack about the method that doesn't fit in this PR. Deferred.

### Out of scope (won't touch)

- VM splits / renames (per the assessment above)
- `PositionTracker` field visibility (legitimately needs external read access from `MediaOpenedCommand`; reducing it would require a larger API change)
- Cosmetic VM rename suggestions

---

## Verification performed

- `dotnet build src/MediaPlayer.sln` — 0 warnings, 0 errors after each change
- Manual verification of MEF composition path for `ViewMediaPlayer` (composed via `container.GetExports<ViewMediaPlayer>()` in `MessengerRegistrations.OpenMainWindow`)
