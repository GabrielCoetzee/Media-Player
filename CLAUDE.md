# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

```bash
# Build the entire solution
dotnet build src/MediaPlayer.sln

# Run all tests
dotnet test src/MediaPlayer.ViewModel.Test

# Run a single test by name
dotnet test src/MediaPlayer.ViewModel.Test --filter "FullyQualifiedName~TestMethodName"

# Clean build
dotnet clean src/MediaPlayer.sln && dotnet build src/MediaPlayer.sln

# Run the application (startup project is MediaPlayer.Shell)
dotnet run --project src/MediaPlayer.Shell
```

## Architecture

WPF desktop media player using **MVVM** on **.NET 10.0-windows**. Personal/non-commercial project.

### Project Dependency Graph

```
MediaPlayer.Shell (entry point, WPF App)
  └─ MediaPlayer.View (XAML views, code-behind)
       └─ MediaPlayer.ViewModel (ViewModels, commands, services)
            ├─ MediaPlayer.Model (domain entities, metadata services, TagLib# integration)
            │    ├─ Integration.LastFM (album art API)
            │    ├─ Integration.LyricsOVH (lyrics API)
            │    └─ MediaPlayer.Common (enums, constants, exceptions — targets net10.0, NOT net10.0-windows)
            └─ MediaPlayer.Settings (config models, settings UI/ViewModels)

Generic (standalone utility library — DI, caching, messaging, named pipes)
  └─ Referenced by: Shell, View, ViewModel, Model, Settings
```

`Directory.Build.props` sets `net10.0-windows` for all projects. Exception: `MediaPlayer.Common` overrides to `net10.0` (no Windows dependencies).

### Dependency Injection — MEF

Uses MEF (`System.ComponentModel.Composition`) not the Microsoft.Extensions.DependencyInjection container. Services and ViewModels are registered via `[Export]` / `[Import]` attributes. Composition happens in `App.xaml.cs` → `MEF.ComposeAll()` → `MEF.Build()` (setup in `Generic/Dependency Injection/MEF.cs`).

### Messaging

Custom `Messenger<T>` (in `Generic/Mediator/Messenger.cs`) decouples components. Message types are defined in `MediaPlayer.Common/Enumerations/MessengerMessages.cs`. Registrations are wired in `MediaPlayer.Shell/Messenger Registrations/MessengerRegistrations.cs` at startup.

**Three notification mechanisms exist — use the right one for the coupling shape:**
- **`PropertyChanged`** — for view bindings only. Never use it as a VM-to-VM communication channel.
- **Messenger** — for broadcast/cross-cutting signals fired from multiple unrelated sources or that cross assembly boundaries (`AutoAdjustAccent`, `AddMedia`, etc.).
- **Dedicated C# events** — for directed, one-to-one VM-to-VM relationships where the coupling is already intentional and explicit (e.g. `MediaControlsViewModel` subscribing to `QueueViewModel.SelectedMediaItemChanged`).

**When adding a new trigger for an existing Messenger message**, add the subscription inside the existing `MessengerRegistrations` method for that message — not a new method. This keeps the complete answer to "what causes X to fire" grouped in one readable place.

### Commands

19 `ICommand` implementations in `MediaPlayer.ViewModel/Commands/`, each exported via MEF with `[ExportMetadata]` string names matching constants in `CommandNames`.

- **Commands receive their target VM via `CommandParameter`, not via `[Import]`.** The View binds `CommandParameter` — single binding (when the DataContext already *is* the target VM) or `MultiBinding` via a converter (when multiple values are needed) — and the command's `Execute` casts that parameter to the VM type. No command in `Commands/Concrete` uses MEF `[Import]` to resolve a VM dependency. Keep new commands consistent with this convention so the call site stays discoverable in XAML rather than hidden in MEF composition.
- **A command's `[Import]` lives on the VM whose state it operates on.** Examples: `StopCommand` / `PlayPauseCommand` on `MediaControlsViewModel`; `ClearMediaListCommand` / `AddMediaCommand` / `RemoveMediaItemCommand` on `QueueViewModel`; `ToggleLyricsCommand` / `EscapeCommand` on `PlayerShellViewModel`. XAML reaches the command via the appropriate property path on the root DataContext (e.g., `{Binding QueueViewModel.AddMediaCommand}`). This rule has no exception for compound parameters — the `MultiBinding` for the `CommandParameter` and the location of the command's `[Import]` are independent concerns. A `MultiBinding` resolves its parameter sources from the binding scope just as easily whether the command lives at the root or one level deeper.

### ViewModel Structure

Six ViewModels form the ViewModel layer:

- **`QueueViewModel`** — track list. Owns `MediaItems`, `SelectedMediaItem`, navigation logic (next/previous/first/last index helpers), the load/enrich orchestration (`AddMediaAsync` → loader batches → updater), persistence of dirty metadata (`SaveDirtyMetadataAsync`), and the clear-list ceremony (`ClearMediaListAsync`). The *mechanism* of streaming load (batching, cancellation) lives in `IMediaLoader`; the *mechanism* of metadata enrichment (parallel HTTP calls, cancellation) lives in `IMetadataUpdateService`. Both services own their own `CancellationTokenSource` lists and expose `Cancel()`; the VM holds no cancellation state. The `SelectedMediaItem` setter raises `PropertyChanged` and fires `SelectedMediaItemChanged` — **no engine side-effects**. Imported by both `MediaControlsViewModel` and `PlayerShellViewModel`.
- **`MediaControlsViewModel`** — player controls brain. Owns `IAudioEngine`, playback state, seek/volume/mute, `IsRepeatEnabled`, `IsShuffled`, and all player commands. Imports `QueueViewModel`; subscribes to `QueueViewModel.SelectedMediaItemChanged` to trigger playback on selection changes.
- **`PlayerShellViewModel`** — app shell. Owns panel states (`IsLyricsOpen`, `IsQueueOpen`, `IsSettingsOpen`), drag/drop, and window lifecycle. Imports all child VMs.
- **`BusyViewModel`** — loading state. Owns `IsLoading` and `MediaListTitle`; exposes named state-transition methods (`MediaListLoading`, `UpdatingMetadata`, `SavingChanges`, etc.) called by `QueueViewModel` during async operations.
- **`ThemeViewModel`** — appearance. Owns `AutoAdjustAccent`, `UseDarkMode`, `BackdropType` settings and exposes `AutoAdjustAccentAsync`, which computes and applies a dominant-color accent from album art bytes.
- **`SettingsViewModel`** — settings aggregator. Wraps `MetadataSettings` properties (`UpdateMetadata`, `SaveMetadataToFile`) and exposes `ThemeViewModel` for the settings panel.

New code touching the queue (items, selection, navigation) → `QueueViewModel`. Playback controls, shuffle/repeat → `MediaControlsViewModel`. Panel visibility, app lifecycle → `PlayerShellViewModel`.

### Playback control

`IAudioEngine` (LibVLC-backed) is mediated by `MediaControlsViewModel`, which is the **sole consumer of `IAudioEngine` in the codebase**.

- **Don't `[Import] IAudioEngine` anywhere except `MediaControlsViewModel`.** Other VMs and commands must route playback ops through `MediaControlsViewModel.{Play, TogglePause, Stop}`. Engine events are consumed internally by `MediaControlsViewModel`: `PositionChanged` updates `SelectedMediaItem.ElapsedTime`, `DurationDiscovered` updates `SelectedMediaItem.Duration`, and `TrackEnded` advances the queue (see below). If a future consumer (e.g. a visualizer) needs these events, re-raise them on `MediaControlsViewModel` at that point — don't pre-emptively expose them.
- **The engine plays one file at a time — it does not own a queue.** `QueueViewModel.MediaItems` is the single source of truth for playback order. Queue navigation (next/previous, repeat-wrap, end-of-queue stop) lives in commands; `MediaControlsViewModel`'s `AudioEngine_TrackEnded` handler delegates to `NextTrackCommand` internally rather than reimplementing the policy. Don't reintroduce `LoadPlaylist`, `PlayAt(int)`, or any other index-shaped or list-shaped API on `IAudioEngine` — it had one and we deliberately removed it (see `docs/plans/done/ENGINE_QUEUE_REMOVAL_PLAN.md`).
- **Selection triggers playback via a dedicated event, not a setter side-effect.** `QueueViewModel.SelectedMediaItem`'s setter raises `PropertyChanged` and fires `SelectedMediaItemChanged` — no engine calls. `MediaControlsViewModel.OnImportsSatisfied` subscribes to `QueueViewModel.SelectedMediaItemChanged`; the handler calls `Play(path)` on non-null selection and `Stop()` on null. Don't add engine calls into the `SelectedMediaItem` setter. **Corollary — "selection mirrors playback":** code that clears `SelectedMediaItem` should expect playback to stop as a side-effect; there is no "deselect-without-stopping" path by design.
- **The engine is authoritative for playback state; VMs project from it.** `MediaControlsViewModel.PlaybackState`'s setter is private — it's only driven by `IAudioEngine.StateChanged`. Don't make a VM "authoritative" for state the engine owns; bidirectional sync creates fragile equality guards and "fake" state when engine actions fail (e.g. a `Play()` call on a corrupt file leaves the property lying).
- **Setter side-effects are for values; action methods are for actions.** `Volume = x` and `SeekPosition = ts` setters that call into the engine are fine — those are values. Play/pause/stop are *actions*, not state, and belong as methods (e.g., `TogglePause()` on the engine, `Play(path)` / `Stop()` on `MediaControlsViewModel`), not setter side-effects on a state property. State-dependent dispatch *inside* a named action method is fine — `TogglePause` reading `PlaybackState` to decide pause-vs-resume is the endorsed pattern. The rule is: no implicit dispatch via property setters. Don't reintroduce a `PlaybackState = Playing` pattern.
- **Engine→orchestrator events that describe per-track state carry the track's path.** `DurationDiscoveredEventArgs` and `TrackEndedEventArgs` each include `Path`; `MediaControlsViewModel` validates against `QueueViewModel.SelectedMediaItem.FilePath.LocalPath` and drops events whose path doesn't match. Defends against races where a VLC callback for a just-superseded track surfaces after the user has moved on. Position events are deliberately *not* path-stamped because they self-correct within one tick. Any new engine→orchestrator event that describes a specific track should follow the same pattern.

### Metadata Pipeline

`IMetadataServices` aggregates four interfaces: `IMetadataReaderService` (TagLib#), `IMetadataWriterService`, `IMetadataUpdateService` (fetches album art from LastFM, lyrics from LyricsOVH), and `IMetadataCorrectorService`. Service implementations live in `MediaPlayer.ViewModel/Services/`.

### Domain Model

`MediaItem` (abstract) → `AudioItem`, `VideoItem`. Built via builder pattern (`AudioItemBuilder`, `VideoItemBuilder`). Collection type: `MediaItemObservableCollection` in `MediaPlayer.Model/Collections/`.

- **`MediaItemObservableCollection.AddRange` / `RemoveRange` raise a single `NotifyCollectionChangedAction.Reset`, not per-item `Add` / `Remove`.** This is inherited from `BulkObservableCollection<T>` in `Generic/Collections/`. Subscribers to `CollectionChanged` cannot rely on `e.NewItems` / `e.OldItems` for batched mutations — `Reset` carries no diff. If you need to react to a specific change, drive the side-effect imperatively at the call site or use per-item `Add` / `Remove` instead of the bulk methods.

### Single Instance & Named Pipes

Mutex-based single-instance check in `App.xaml.cs`. Subsequent launches forward file arguments to the running instance via `NamedPipeManager` (`Generic/Named Pipes/`).

Because the app is single-instance, every MEF singleton (commands, ViewModels, services) is process-wide and exists exactly once. State held on a command instance (e.g., a cached event-handler reference for an unsubscribe/resubscribe pattern) is therefore safe — there is no second instance to interfere with it.

## Testing

- **Framework:** NUnit 4.x with Moq
- **Test project:** `MediaPlayer.ViewModel.Test`
- Test data files (MP3s, cover art) are in `_Test Files/Input Files/` and copied to output via `PreserveNewest`

## Key Libraries

| Library | Purpose |
|---------|---------|
| Wpf.Ui | WPF theming, accent color management, backdrop types |
| TagLibSharp | Audio file metadata read/write |
| SixLabors.ImageSharp | Image processing (dominant color extraction for auto-accent) |
| Flurl.Http | HTTP client for external API calls |
| Newtonsoft.Json | JSON serialization |
| FontAwesome5 | Icon fonts in UI |

## Code Conventions
- Always use Flurl, but don't use the `new FlurlRequest`, use '.AppendPathSegments` etc. directly on the URL string.
- One class per file.
- Separate interfaces and base classes from their concrete implementations. Place abstractions in an `Abstract` folder and concrete implementations in a `Concrete` folder.
- **ViewModel vs Model placement:** If a class mutates a domain entity (`MediaItem`, `AudioItem`, `VideoItem` — e.g., sets lyrics, album art, dirty flags), it belongs in `MediaPlayer.Model`. If it computes or orchestrates without touching the entities, it belongs in `MediaPlayer.ViewModel`. Example: `LyricsCorrector` mutates `AudioItem.Lyrics` → Model; `ImageSharpColorService` returns a `Color` from image bytes → ViewModel.
