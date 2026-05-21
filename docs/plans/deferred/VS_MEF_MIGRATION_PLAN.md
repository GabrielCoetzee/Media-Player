# Migrate from .NET MEF to VS-MEF (`Microsoft.VisualStudio.Composition`)

## Context

Investigation confirmed two long-standing intuitions: the `CompositionContainer` in `System.ComponentModel.Composition` (the classic .NET MEF) retains strong references to `IDisposable` + `NonShared` parts (well-documented leak), and **VS-MEF** (`Microsoft.VisualStudio.Composition`, latest 17.13.41, May 2025) is an actively maintained reimplementation by the Visual Studio team.

This codebase doesn't trigger the leak conditions (no `NonShared` parts, no `IDisposable` exports, secondary windows bypass MEF). The migration is being undertaken anyway for the broader benefits: precomputed composition graph (faster `GetExports`), better composition diagnostics, optional serializable catalog for fast startup, and ongoing maintenance. VS-MEF keeps the same `[Export]`/`[Import]`/`[ImportingConstructor]`/`[PartCreationPolicy]` attributes via `AttributedPartDiscoveryV1`, so the per-class part definitions don't move — only the container bootstrap and the ~5 runtime call sites need to change.

### Why VS-MEF and not `Microsoft.Extensions.DependencyInjection`

`M.E.DI` is already partially in the project (`Generic.csproj:9`, used only for an `IWritableOptions<T>` registration). It would be the more "modern" .NET migration target. It was explicitly rejected: M.E.DI does not support property injection, which this codebase relies on (e.g. `App.ThemeSettings`, `ViewMediaPlayer.ViewModel`, plus many ViewModels). Converting to constructor-only injection would balloon constructor signatures and forced ordering across a large surface. VS-MEF preserves the property-injection model unchanged. The M.E.DI option is parked for possible revisit later.

### Maintenance reassurance (as of May 2026)

VS-MEF is actively maintained — `microsoft/vs-mef` has multiple commits in April 2026 (latest 2026-04-30, mostly renovate-bot dependency rolls and Library.Template syncs). Latest NuGet release `17.13.41` was published 2025-05-14, so ~12 months ago; cadence has slowed but the project is not dormant. The `net8.0`-only target reflects Visual Studio 2022's runtime (VS-MEF tracks VS, not the latest .NET) and consumes forward-compatibly under `net10.0-windows`. No deprecation signals.

## What stays the same

- All 53 files that use `using System.ComponentModel.Composition;` and the attribute set (`[Export]`, `[Import]`, `[ImportingConstructor]`, `[PartCreationPolicy]`, `[ExportMetadata]`). VS-MEF reads these via `AttributedPartDiscoveryV1`.
- Project graph and DLL layout. Parts are still loaded from all DLLs in the app directory (and subdirectories), mirroring the current `AssemblyCatalog` + `DirectoryCatalog` behavior.
- The `Generic.Concrete.WindowService<T>` pattern (secondary windows via `new T()`) — untouched.
- The test project (`MediaPlayer.ViewModel.Test`) doesn't reference MEF, no changes there.

## What changes

### 1. Package references

**`src/Generic/Generic.csproj`** (currently has `System.ComponentModel.Composition` 10.0.7)
- **Add:** `<PackageReference Include="Microsoft.VisualStudio.Composition" Version="17.13.41" />`
- **Keep** `System.ComponentModel.Composition` 10.0.7 — code still uses the attributes from that assembly. VS-MEF declares it as a dependency anyway; keeping it explicit is clearer.

**`src/MediaPlayer.View/MediaPlayer.View.csproj`** (currently has `System.ComponentModel.Composition` 10.0.7)
- **Keep** `System.ComponentModel.Composition` 10.0.7 — `ViewMediaPlayer.xaml.cs` uses `[Export]`/`[Import]`/`[ImportingConstructor]` attributes from it. No VS-MEF reference needed here (only the composition root needs it).

VS-MEF officially ships `net8.0` / `netstandard2.0` / `net472` binaries; `net10.0-windows` consumes these forward-compatibly (NuGet computed-compatible). This works in practice — the package's reflection-based discovery is TFM-agnostic.

### 2. Rewrite `src/Generic/Dependency Injection/MEF.cs`

Replace the class entirely with a VS-MEF-based composition root that exposes an `ExportProvider` named `Container` (so the rest of the codebase reads naturally), provides an async `ComposeAllAsync(Assembly)` bootstrap method, and a `Dispose()` method for shutdown.

Notes:
- Public surface: `Container` (typed as `ExportProvider`, not `CompositionContainer`) and `ComposeAllAsync(Assembly)`. The name `Container` is preserved so the rest of the codebase reads naturally — callers don't care that it's a different type as long as they call `GetExportedValue<T>()` on it.
- `AttributedPartDiscoveryV1` is the discovery class for the existing attributes — confirmed against the VS-MEF hosting docs.
- `config.ThrowOnErrors()` surfaces composition errors at startup with detailed diagnostics (the main VS-MEF UX win over .NET MEF, where bad compositions silently dropped parts).
- The unused `Compose(Assembly, string)` overload (current `MEF.cs:31-47`) is deleted — confirmed no callers.
- `MEF.Build(Application)` is deleted (see step 3 for why).
- A private helper enumerates the app directory (and subdirectories) for `*.dll`, attempts `Assembly.LoadFrom`, and swallows `BadImageFormatException` / `FileLoadException` for unmanaged or unloadable assemblies — mirroring the resilience `DirectoryCatalog` provided implicitly.

### 3. Replace `MEF.Build(this)` with explicit lookup in `src/MediaPlayer.Shell/App.xaml.cs`

The current `Build(app)` does `Container.ComposeParts(app)` which satisfies the `[Import] ThemeSettings` property on `App` (`App.xaml.cs:25-26`). VS-MEF doesn't expose an equivalent "satisfy imports on a pre-existing object" call in its public API.

- Remove the `[Import]` attribute from `App.ThemeSettings` and make the setter `private`.
- After composition, fetch via `MEF.Container.GetExportedValue<ThemeSettings>()`.
- `InitializeMEF` becomes `InitializeMEFAsync` and is awaited from `OnStartup` (which is already `async override void`).
- Override `OnExit` to call `MEF.Dispose()` so the `ExportProvider` is released on shutdown.

### 4. Update `src/MediaPlayer.Shell/Messenger Registrations/MessengerRegistrations.cs`

- Change all four parameter types from `CompositionContainer` to `ExportProvider`.
- Change `using System.ComponentModel.Composition.Hosting;` to `using Microsoft.VisualStudio.Composition;`.
- Replace the 5 call sites (`MessengerRegistrations.cs:20, 30, 40, 55, 57`):
  - `container?.GetExports<T>().Single().Value` → `container?.GetExportedValue<T>()`

No call-site changes elsewhere — `App.xaml.cs:47-50` passes `MEF.Container`, which is now an `ExportProvider`. Types line up.

### 5. Error-handling tweak in `App.xaml.cs:98-104`

Current code catches `ReflectionTypeLoadException` from MEF startup. VS-MEF's `config.ThrowOnErrors()` throws `CompositionFailedException` (with structured diagnostics) instead. The catch is broadened to include both, so failures still surface to the user via `MessageBox`.

## Files changing

| File | Change |
|---|---|
| `src/Generic/Generic.csproj` | + `Microsoft.VisualStudio.Composition` 17.13.41 |
| `src/Generic/Dependency Injection/MEF.cs` | Rewrite: `ExportProvider`-based, async `ComposeAllAsync`, drop unused `Compose` overload, add `Dispose` |
| `src/MediaPlayer.Shell/App.xaml.cs` | `[Import]` → explicit `GetExportedValue`, async init, `OnExit` disposes container, broaden composition-error catch |
| `src/MediaPlayer.Shell/Messenger Registrations/MessengerRegistrations.cs` | `CompositionContainer` → `ExportProvider`, `GetExports<T>().Single().Value` → `GetExportedValue<T>()` |

No other files need to change — all 53 `[Export]` / `[Import]` declarations across the solution stay as-is.

## Verification

1. **Build:** `dotnet build src/MediaPlayer.sln` — must succeed with no errors. Pay attention to any new analyzer warnings from `Microsoft.VisualStudio.Composition.Analyzers` (it ships with the package); they flag ambiguous or invalid compositions at compile time.
2. **Tests:** `dotnet test src/MediaPlayer.ViewModel.Test` — must pass. Tests don't use MEF, so this is mostly a confidence check that nothing else was broken in the touched assemblies.
3. **Composition validation at startup:** the app should start without throwing `CompositionFailedException`. If it does, the exception's `Errors` collection prints the precise reason — this is the diagnostic improvement over .NET MEF.
4. **Run the app:** `dotnet run --project src/MediaPlayer.Shell`. Sanity-check the golden paths that exercise the previous `GetExports` call sites:
   - Main window opens (uses `GetExportedValue<ViewMediaPlayer>` and `GetExportedValue<MainViewModel>`).
   - Drag-and-drop / file-args adds media (`AddMedia` messenger).
   - Save-on-close commits dirty edits (`SaveChangesToDirtyFiles` messenger).
   - Auto-accent updates on track change (`AutoAdjustAccent` messenger — exercises both `MainViewModel` and `ThemeViewModel` lookups).
   - Dark/light theme — confirms the explicit `ThemeSettings` lookup replacing the old `[Import]` works.
   - Settings window opens (still via `WindowService<T>` / `new T()` — unrelated to MEF, but worth confirming we didn't accidentally regress it).
5. **Shutdown:** close the main window. `OnExit` should dispose `MEF.Container` without exceptions.

## Risks and notes

- **TFM compatibility:** VS-MEF ships `net8.0` binaries; this solution targets `net10.0-windows`. NuGet treats this as forward-compatible. If anything misbehaves at runtime, the likely surface is reflection over types that have changed shape across .NET versions — unlikely here, but worth keeping in mind if a discovery error appears.
- **Async-void `OnStartup`:** the method is already `async override void`, so awaiting `ComposeAllAsync` is a no-op style change. No threading model change.
- **Assembly load order:** VS-MEF discovers eagerly (vs the current lazy `DirectoryCatalog`). Some DLLs in the output directory (Velopack updater bits, Wpf.Ui resources, native interop libs) may throw `BadImageFormatException` / `FileLoadException` when `Assembly.LoadFrom` hits them — the new `LoadAssembliesFromAppDirectory` swallows those specific exceptions and continues.
- **`MainWindow` reference:** `App.xaml.cs:65` casts `Current.MainWindow` to `ViewMediaPlayer`. The main window becomes `Current.MainWindow` automatically when shown by WPF — unchanged by this migration, but worth verifying after.
- **Out of scope (deliberate):** the optional `CachedComposition` (serializable catalog, faster startup) is not added. For 9 exports it's not worth the complexity. Easy to add later if startup time ever becomes a concern.
