# UI Modernization Plan

This document is the agreed plan for the WPF UI rework on branch `feat/wpf-ui-modernization`. It captures the decisions reached during design, the implementation order, what's intentionally out of scope, and the risks we accepted going in. Refer back here whenever scope creep or "wait, did we agree on X?" comes up.

---

## Decisions Locked

| # | Decision |
|---|---|
| 1 | **Framework:** Drop MahApps Metro + FontAwesome5; adopt WPF-UI (`Wpf.Ui` NuGet). |
| 2 | **Layout:** Single-page hub. Settings as anchored flyout. Queue/lyrics as togglable `SplitView` panes. |
| 3 | **Macro layout:** Audio-hero with adaptive video — same frame swaps album art ↔ `MediaElement` via `IsAudio` `DataTrigger`. |
| 4 | **Chrome:** WPF-UI `FluentWindow` with `ExtendsContentIntoTitleBar`, `<ui:TitleBar>` for system buttons + custom controls in `TrailingContent`. |
| 5 | **Settings:** Anchored `Flyout` from gear button; auto-save on change (no Save/Close button); flat `<ui:CardExpander>` groups instead of `TabControl`. |
| 6 | **Volume:** Continuous `double` slider; speaker icon as mute toggle; remember pre-mute level. |
| 7 | **Queue:** Individual remove (X on hover, `e.Handled = true` so single-click-row-to-play stays); drag-to-reorder with `Id` reassignment after `Move()`; single-select preserved. |
| 8 | **Migration:** Single big-bang on `feat/wpf-ui-modernization`. |
| 9 | **Defaults:** Queue open, lyrics closed; min window 900×600; SymbolIcon + ProgressRing everywhere; vector default-art placeholder; `+` in queue header, "Clear All" in queue overflow `…`; dynamic title binding kept. |
| 10 | **Accent:** System accent + auto-from-art; manual accent picker deleted. |
| 11 | **Polish:** Software keyboard shortcuts (no seek shortcuts); empty-state UserControl; SMTC and mini-player explicitly **deferred**. |

---

## Project Structure (target)

```
MediaPlayer.View/
│
├── Views/
│   └── ViewMediaPlayer.xaml/.cs        ← thin FluentWindow shell (TitleBar + content area)
│
├── Components/                          ← all the modular UserControls
│   ├── PlayerShell.xaml/.cs            ← SplitView orchestration
│   ├── HeroArea.xaml/.cs               ← AlbumArt vs MediaElement, switches on IsAudio
│   ├── AlbumArtDisplay.xaml            ← Image with vector placeholder fallback
│   ├── NowPlayingMetadata.xaml         ← title / artist · album · year stack
│   ├── TransportBar.xaml               ← prev/play/stop/next + seek + shuffle/repeat + volume
│   ├── SeekStrip.xaml                  ← slider + elapsed/total time
│   ├── VolumeControl.xaml              ← speaker icon (mute toggle) + slider
│   ├── QueuePanel.xaml/.cs             ← header (+ button, … overflow), ListView body
│   ├── QueueItemTemplate.xaml          ← thumb + title + artist + duration + remove-X + now-playing indicator
│   ├── LyricsPanel.xaml                ← header + scrollable lyrics
│   ├── SettingsFlyout.xaml/.cs         ← bound to SettingsViewModel; CardExpander groups
│   └── EmptyState.xaml                 ← "Drop files here or click +" CTA
│
├── Converters/
│   ├── AlbumArtMultiValueConverter.cs        ← KEPT (simplified)
│   ├── ElapsedTimeTimeSpanToSecondsConverter.cs ← KEPT
│   ├── MediaElementOpenedMultiValueConverter.cs ← KEPT
│   └── BooleanToVisibleOrHiddenInverseConverter.cs ← KEPT
│
├── Themes/
│   └── PlaceholderArt.xaml             ← vector default-album-art template
│
├── Behaviors/
│   └── ListBoxDragReorderBehavior.cs   ← attached behavior wrapping Move() + Id-reassignment
│
└── Extensions/
    └── MediaElementExtension.cs        ← KEPT (Position attached property)
```

The split between `ViewMediaPlayer.xaml` (thin window shell) and `PlayerShell.xaml` (UserControl with the actual UI) is the **NavigationView-readiness gate**: when library/playlists/EQ get added later, the window's content area swaps to `<ui:NavigationView>` and `PlayerShell` becomes one nav destination — zero rewrite of any component.

---

## Surface Changes Outside the View Layer

These are the **only** changes outside `MediaPlayer.View/`. Everything else is view-side.

### `MediaPlayer.ViewModel`
- `MediaControlsViewModel.MediaVolume`: `VolumeLevel` → `double` (0.0–1.0); add private `_preMuteVolume` field.
- `Commands/Concrete/MuteCommand.cs`: track pre-mute level for restore.
- `Commands/Concrete/OpenSettingsWindowCommand.cs`: **delete** (gear button binds Flyout `IsOpen` directly in XAML).
- `MainViewModel.OpenSettingsWindowCommand` `[Import]`: **delete**.

### `MediaPlayer.Settings`
- `ThemeViewModel`: keep public surface for `UseDarkMode`, `AutoAdjustAccent`, `BackdropType`, `IsBackdropSupported`, `BackgroundColor` / `ForegroundColor` / `EffectiveBackgroundColor`, `AutoAdjustAccentAsync(byte[])`. Internals swap from `ControlzEx.Theming.ThemeManager` + `RuntimeThemeGenerator` → `Wpf.Ui.Appearance.ApplicationThemeManager` + `Wpf.Ui.Appearance.ApplicationAccentColorManager`.
- `ThemeViewModel.Accent`, `AccentLabel`: **delete**.
- `Commands/LoadAccentOptionsCommand.cs`: **delete**.
- `Commands/SaveSettingsCommand.cs`: **delete** (auto-save on change).
- `SettingsViewModel.LoadAccentOptionsCommand`, `SaveSettingsCommand` `[Import]`s: **delete**.
- `ThemeSettings.Accent` field: keep for now (Json compat) but unused — schedule for cleanup later.

### `MediaPlayer.Common`
- `Enumerations/VolumeLevel.cs`: **delete**.
- `MessengerMessages.OpenApplicationSettingsDialog`: **delete**.
- `MessengerMessages.ApplyDwmBackdrop`: **delete**.
- `Constants/CommandNames.OpenSettingsWindow`, `LoadAccentOptionsCommand`, `SaveSettings`: **delete**.

### `MediaPlayer.Shell`
- `App.xaml`: replace MahApps merged dictionaries with WPF-UI's `<ui:ThemesDictionary />` + `<ui:ControlsDictionary />`.
- `App.xaml.cs::LoadTheme()`: rewrite to call `ApplicationThemeManager.Apply(...)` + `ApplicationAccentColorManager.Apply(...)`.
- `MessengerRegistrations.OpenApplicationSettingsDialog` and `ApplyDwmBackdrop` registrations: **delete**.

---

## NuGet Changes

| Package | Change |
|---|---|
| `MahApps.Metro` | **Remove** |
| `ControlzEx` | **Remove** (transitive of MahApps; explicit if pinned) |
| `FontAwesome5` (and `FontAwesome5.Net`) | **Remove** |
| `Wpf.Ui` | **Add** (latest stable — currently 3.x) |

`SixLabors.ImageSharp`, `TagLibSharp`, `Flurl.Http`, `Newtonsoft.Json`, `Microsoft.Xaml.Behaviors.Wpf`, `System.ComponentModel.Composition` all stay.

---

## Theming Migration

```csharp
// Before (MahApps):
var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(BaseColor, dominantColor);
ThemeManager.Current.AddTheme(theme);
ThemeManager.Current.ChangeTheme(Application.Current, theme);

// After (WPF-UI):
ApplicationAccentColorManager.Apply(
    dominantColor,
    UseDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light);
```

`ImageSharpColorService.GetDominantColorAsync` already returns a `System.Windows.Media.Color` — drop-in compatible. Auto-accent flow stays semantically identical; it just talks to WPF-UI's accent system instead of MahApps's.

DWM backdrop: `FluentWindow.WindowBackdropType` exposes `Auto/None/Mica/Acrylic/Tabbed`. `IsBackdropSupported` becomes a no-op (`Auto` gracefully degrades on unsupported OS versions).

---

## Implementation Order (commit sequence)

1. **Foundation swap** — NuGet swaps. `App.xaml` resource dictionary swap. Compile breaks; that's expected.
2. **Theme bridge** — `ThemeViewModel` internals rewrite. `App.xaml.cs::LoadTheme` rewrite. Delete `DwmBackdropService` + `WindowResolutionCalculator`. Delete now-orphan `MessengerRegistrations`.
3. **VM cleanup** — Volume `VolumeLevel` → `double`. `MuteCommand` updates. Delete `VolumeLevel`. Delete `MediaVolumeConverter`. Delete `OpenSettingsWindowCommand` + import. Delete `Accent` / `AccentLabel` / `LoadAccentOptionsCommand` / `SaveSettingsCommand`.
4. **Window shell** — `ViewMediaPlayer.xaml` rewritten as `<ui:FluentWindow>` with `<ui:TitleBar>` + content area hosting `<components:PlayerShell />`. App now compiles + opens, content area empty.
5. **EmptyState + PlayerShell skeleton** — App now shows "Drop files here" on launch.
6. **TransportBar + SeekStrip + VolumeControl** — full transport functionality.
7. **HeroArea + AlbumArtDisplay + NowPlayingMetadata + PlaceholderArt** — track info renders, art shows.
8. **QueuePanel + QueueItemTemplate + ListBoxDragReorderBehavior** — queue UX complete (single-click play, X-on-hover remove, drag-reorder, single-select).
9. **LyricsPanel** — toggleable left pane.
10. **SettingsFlyout** — gear-anchored flyout with CardExpander groups.
11. **Cleanup pass** — delete `ViewApplicationSettings.xaml/.cs`, remaining dead converters, `Resources/Button_Images/`, `Resources/Default_AlbumArt/`, dead `MessengerMessages` enum values, dead `CommandNames` constants.
12. **Verification** — keyboard shortcuts work, theme switching live, auto-accent fires on track change, video playback works in adaptive frame, drag-drop on window still works, drag-reorder in queue works, file-arg forwarding (single-instance) still works.

---

## Explicit Non-Goals

- **SMTC integration** — deferred (separate effort, VM-level work, requires WinRT interop).
- **Mini-player mode** — deferred. The component decomposition makes this an easy follow-up: a separate compact window can reuse `TransportBar`, `AlbumArtDisplay`, and `NowPlayingMetadata` directly.
- **Right-click context menu on queue items** — deferred. Would need to rethink the single-click-row-to-play interaction first.
- **Multi-select in queue** — kept single-select per design decision.
- **Library / playlists / search / EQ / visualizer** — future work. NavigationView migration when these arrive.
- **Audio-format properties (bitrate, sample rate, resolution) display** — not exposed by the model today; deferred.

---

## Risks Acknowledged Going In

1. **`<ui:TitleBar>` interaction with single-instance file-arg forwarding.** `BringToForeground()` (`ViewMediaPlayer.xaml.cs:51-63`) sets `Topmost = true; Topmost = false;`. Should still work on `FluentWindow` (which inherits `Window`), but smoke-test.
2. **`MediaElement` inside `SplitView.Content` with backdrop.** WPF's `MediaElement` is hwnd-based and renders above XAML composition. With Mica backdrop, this *may* show as a hard rectangle that breaks the unified Mica look during video playback. If it does, fallbacks: (a) accept the visible boundary during video, (b) try `MediaPlayerElement` from WPF-UI if available, (c) `Window.GlassFrameThickness` workarounds.
3. **`AutoScrollListView` (Generic library) + drag-reorder behavior.** Auto-scroll-to-selected and drag-to-reorder both manipulate scroll/index. They might fight. Test in step 8.
4. **MEF composition with `[Export] FluentWindow`.** Existing pattern `[Export] public partial class ViewMediaPlayer : MetroWindow` should port cleanly to `FluentWindow`, but MEF + WPF-UI's window class is unverified. Smoke-test in step 4.
