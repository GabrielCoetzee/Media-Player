# DWM Backdrop Transparency — Implementation Plan

## Context

`AllowsTransparency="True"` was removed from both `MetroWindow` instances because MahApps.Metro 3.x
(upgraded from 2.4.x as part of the .NET 10 migration) is incompatible with it — the window's internal
`WindowStyle` is no longer `None`, causing a runtime `InvalidOperationException`. The old feature allowed
the window background to appear semi-transparent over the desktop via a brush opacity binding.

This plan restores that feature using the Windows DWM backdrop API
(`DwmSetWindowAttribute` with `DWMWA_SYSTEMBACKDROP_TYPE`), which is the correct modern replacement
and works correctly with MahApps.Metro 3.x / ControlzEx 7.x.

---

## Overview

**11 files changed** (3 new, 8 modified).
No changes to `App.xaml`, `App.xaml.cs`, `MessengerRegistrations.cs`, or any ViewModel other than
`ThemeViewModel`.

---

## Task 1 — New enum `DwmBackdropType` in `MediaPlayer.Common`

**New file:** `MediaPlayer.Common/Enumerations/DwmBackdropType.cs`

Maps directly to `DWM_SYSTEMBACKDROP_TYPE` values:

```csharp
public enum DwmBackdropType
{
    Auto    = 0, // DWMSBT_AUTO    — system decides (not recommended for explicit control)
    None    = 1, // DWMSBT_NONE    — no backdrop, solid MahApps theme colour
    Mica    = 2, // DWMSBT_MAINWINDOW    — Mica material (requires Win 11 22H2+)
    Acrylic = 3, // DWMSBT_TRANSIENTWINDOW — Acrylic translucent (requires Win 11 22H2+)
    Tabbed  = 4  // DWMSBT_TABBEDWINDOW  — Mica Alt (requires Win 11 22H2+)
}
```

Lives in `MediaPlayer.Common` because both `MediaPlayer.Settings` (persisted in `ThemeSettings`) and
`MediaPlayer.View` (consumed by the backdrop service) need it.

---

## Task 2 — Add `ApplyDwmBackdrop` to `MessengerMessages` enum

**Modify:** `MediaPlayer.Common/Enumerations/MessengerMessages.cs`

Additive only — one new entry:

```csharp
ApplyDwmBackdrop
```

Sent by `ThemeViewModel` when the user changes the backdrop setting. Consumed by the main window
code-behind to re-apply it to the HWND live. The settings window does not need to subscribe.

---

## Task 3 — Add `DwmBackdropService` to `ServiceNames` constants

**Modify:** `MediaPlayer.Common/Constants/ServiceNames.cs`

```csharp
public const string DwmBackdropService = nameof(DwmBackdropService);
```

Follows the exact pattern of `ImageSharpColorService` and `HardCodedWindowResolutionCalculator`.

---

## Task 4 — Add `BackdropType` property to `ThemeSettings`

**Modify:** `MediaPlayer.Settings/Configuration/ThemeSettings.cs`

```csharp
public DwmBackdropType BackdropType { get; set; } = DwmBackdropType.Acrylic;
```

The old `Opacity` property is retained for backwards-compatible deserialisation — Newtonsoft.Json silently
uses the default value for properties missing from existing settings files on disk. `Opacity` is unused by
the ViewModel after Task 6 but kept in the model to avoid a deserialisation error on first launch after the
upgrade.

---

## Task 5 — Create `IDwmBackdropService` + `DwmBackdropService`

**New files:**
- `MediaPlayer.View/Services/Abstract/IDwmBackdropService.cs`
- `MediaPlayer.View/Services/Concrete/DwmBackdropService.cs`

### Interface

```csharp
public interface IDwmBackdropService
{
    bool IsSupported { get; }
    void ApplyBackdrop(Window window, DwmBackdropType backdropType);
    void RemoveBackdrop(Window window);
}
```

### Concrete implementation

All P/Invoke declarations are colocated inside the class (no separate P/Invoke file):

```csharp
[DllImport("dwmapi.dll")]
private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

[DllImport("dwmapi.dll")]
private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

[StructLayout(LayoutKind.Sequential)]
private struct MARGINS { public int Left, Right, Top, Bottom; }

private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
```

**`IsSupported`** — gates on Windows 11 22H2+ (build ≥ 22621):

```csharp
public bool IsSupported
{
    get
    {
        var v = Environment.OSVersion.Version;
        return v.Major >= 10 && v.Build >= 22621;
    }
}
```

**`ApplyBackdrop`:**
1. Get HWND via `new WindowInteropHelper(window).Handle`
2. Return early if `!IsSupported`
3. Call `DwmExtendFrameIntoClientArea` with `MARGINS { -1, -1, -1, -1 }` — prerequisite for any DWM backdrop to show through the WPF client area
4. Cast `backdropType` to `int` and call `DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ...)`

**`RemoveBackdrop`:** Resets `DWMWA_SYSTEMBACKDROP_TYPE` to `1` (None) and un-extends the frame with zero margins.

**MEF export:**
```csharp
[Export(ServiceNames.DwmBackdropService, typeof(IDwmBackdropService))]
```

---

## Task 6 — Refactor `ThemeViewModel` — replace `Opacity` with `BackdropType`

**Modify:** `MediaPlayer.Settings/ViewModels/ThemeViewModel.cs`

**Remove:**
- `public decimal Opacity { get; set; }` property and its `ChangeOpacity()` call
- `public void ChangeOpacity() => Application.Current.MainWindow.Background.Opacity = (double)Opacity;`

**Add:**

```csharp
public DwmBackdropType BackdropType
{
    get => _themeSettings.BackdropType;
    set
    {
        _themeSettings.BackdropType = value;
        OnPropertyChanged(nameof(BackdropType));
        OnPropertyChanged(nameof(EffectiveBackgroundColor));
        Messenger<MessengerMessages>.Send(MessengerMessages.ApplyDwmBackdrop, value);
    }
}

// Returns Transparent when a DWM backdrop is active so the compositor shows through.
// Returns the solid theme colour when backdrop is None or unsupported.
public Color EffectiveBackgroundColor =>
    BackdropType != DwmBackdropType.None
        ? Colors.Transparent
        : UseDarkMode ? Colors.Black : Colors.White;
```

`OnPropertyChanged(nameof(EffectiveBackgroundColor))` must also be fired from the `UseDarkMode` setter.

---

## Task 7 — Update both window XAML Background bindings

**Modify:** `MediaPlayer.View/Views/ViewMediaPlayer.xaml` and `ViewApplicationSettings.xaml`

Replace the `SolidColorBrush` with `Opacity` binding on both windows:

```xml
<!-- Before -->
<Controls:MetroWindow.Background>
    <SolidColorBrush Color="{Binding ...BackgroundColor}" Opacity="{Binding ...Opacity}"/>
</Controls:MetroWindow.Background>

<!-- After -->
<Controls:MetroWindow.Background>
    <SolidColorBrush Color="{Binding ...EffectiveBackgroundColor, FallbackValue=Black}"/>
</Controls:MetroWindow.Background>
```

When a DWM backdrop is active `EffectiveBackgroundColor` is `Transparent`, letting the compositor show
Mica/Acrylic through the client area. When not, it returns the solid theme colour. No `Opacity` attribute
needed.

---

## Task 8 — Wire the service in both view code-behinds

**Modify:** `MediaPlayer.View/Views/ViewMediaPlayer.xaml.cs` and `ViewApplicationSettings.xaml.cs`

Both files get an MEF import and a `Loaded` subscription:

```csharp
[Import(ServiceNames.DwmBackdropService)]
public IDwmBackdropService DwmBackdropService { get; set; }

// In constructor, after SatisfyImportsOnce:
Loaded += (_, _) => DwmBackdropService.ApplyBackdrop(this, /* current BackdropType from ViewModel */);
```

**`ViewMediaPlayer.xaml.cs` only** — also subscribe to the messenger for live changes when the user
adjusts the setting at runtime:

```csharp
Messenger<MessengerMessages>.Register(MessengerMessages.ApplyDwmBackdrop, args =>
{
    if (args is DwmBackdropType backdropType)
        DwmBackdropService.ApplyBackdrop(this, backdropType);
});
```

The settings window does not need the messenger subscription — it opens, reads the current value, and closes.

---

## Task 9 — Replace the opacity slider in the settings UI

**Modify:** `MediaPlayer.View/Views/ViewApplicationSettings.xaml`

The `Slider` becomes a `ComboBox` with three static items bound to `ThemeViewModel.BackdropType` via
`SelectedValue`/`Tag`. Add `xmlns:enums` pointing at `MediaPlayer.Common.Enumerations`:

```xml
<Label Grid.Column="0" Grid.Row="4" Content="Window Backdrop:"
       HorizontalAlignment="Center" VerticalAlignment="Center" />
<ComboBox Grid.Row="4" Grid.Column="1" Grid.ColumnSpan="3"
          SelectedValue="{Binding ThemeViewModel.BackdropType}"
          SelectedValuePath="Tag"
          IsEnabled="{Binding ThemeViewModel.IsBackdropSupported}">
    <ComboBoxItem Content="None (Solid)"  Tag="{x:Static enums:DwmBackdropType.None}" />
    <ComboBoxItem Content="Mica"          Tag="{x:Static enums:DwmBackdropType.Mica}" />
    <ComboBoxItem Content="Acrylic"       Tag="{x:Static enums:DwmBackdropType.Acrylic}" />
</ComboBox>
```

The ComboBox is disabled when `DwmBackdropService.IsSupported` is `false`. Expose
`IsBackdropSupported` as a passthrough property on `ThemeViewModel` (importing the service
or receiving the value from the ViewModel layer).

---

## Task 10 — Full build + test

1. `dotnet build MediaPlayer.sln` — 0 errors, 0 warnings
2. `dotnet test MediaPlayer.sln` — 34/34 pass
3. Run the application on Windows 11 22H2+, exercise the backdrop switcher at runtime

---

## Windows version fallback behaviour

| OS | Behaviour |
|---|---|
| Windows 11 22H2+ (build ≥ 22621) | Full support — None / Mica / Acrylic all work |
| Windows 11 21H2 (builds 22000–22620) | `DwmSetWindowAttribute` returns `S_FALSE` silently; solid theme colour shown |
| Windows 10 | `IsSupported = false`; ComboBox disabled in settings; solid colour shown |

---

## File change summary

| File | Status | Change |
|---|---|---|
| `MediaPlayer.Common/Enumerations/DwmBackdropType.cs` | **NEW** | Backdrop type enum |
| `MediaPlayer.Common/Enumerations/MessengerMessages.cs` | modify | Add `ApplyDwmBackdrop` entry |
| `MediaPlayer.Common/Constants/ServiceNames.cs` | modify | Add `DwmBackdropService` constant |
| `MediaPlayer.Settings/Configuration/ThemeSettings.cs` | modify | Add `BackdropType` property |
| `MediaPlayer.Settings/ViewModels/ThemeViewModel.cs` | modify | Replace `Opacity`/`ChangeOpacity` with `BackdropType` + `EffectiveBackgroundColor` |
| `MediaPlayer.View/Services/Abstract/IDwmBackdropService.cs` | **NEW** | Service interface |
| `MediaPlayer.View/Services/Concrete/DwmBackdropService.cs` | **NEW** | P/Invoke + service implementation |
| `MediaPlayer.View/Views/ViewMediaPlayer.xaml` | modify | Background binding update |
| `MediaPlayer.View/Views/ViewMediaPlayer.xaml.cs` | modify | Import service, wire `Loaded` + messenger |
| `MediaPlayer.View/Views/ViewApplicationSettings.xaml` | modify | Replace slider with ComboBox, Background binding |
| `MediaPlayer.View/Views/ViewApplicationSettings.xaml.cs` | modify | Import service, wire `Loaded` |
