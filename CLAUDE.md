# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

```bash
# Build the entire solution
dotnet build MediaPlayer.sln

# Run all tests
dotnet test MediaPlayer.ViewModel.Test

# Run a single test by name
dotnet test MediaPlayer.ViewModel.Test --filter "FullyQualifiedName~TestMethodName"

# Clean build
dotnet clean MediaPlayer.sln && dotnet build MediaPlayer.sln

# Run the application (startup project is MediaPlayer.Shell)
dotnet run --project MediaPlayer.Shell
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

### Commands

18 `ICommand` implementations in `MediaPlayer.ViewModel/Commands/`, each exported via MEF with `[ExportMetadata]` string names matching constants in `CommandNames`.

### Metadata Pipeline

`IMetadataServices` aggregates four interfaces: `IMetadataReaderService` (TagLib#), `IMetadataWriterService`, `IMetadataUpdateService` (fetches album art from LastFM, lyrics from LyricsOVH), and `IMetadataCorrectorService`. Service implementations live in `MediaPlayer.ViewModel/Services/`.

### Domain Model

`MediaItem` (abstract) → `AudioItem`, `VideoItem`. Built via builder pattern (`AudioItemBuilder`, `VideoItemBuilder`). Collection type: `MediaItemObservableCollection` in `MediaPlayer.Model/Collections/`.

### Single Instance & Named Pipes

Mutex-based single-instance check in `App.xaml.cs`. Subsequent launches forward file arguments to the running instance via `NamedPipeManager` (`Generic/Named Pipes/`).

## Testing

- **Framework:** NUnit 4.x with Moq
- **Test project:** `MediaPlayer.ViewModel.Test`
- Test data files (MP3s, cover art) are in `_Test Files/Input Files/` and copied to output via `PreserveNewest`

## Key Libraries

| Library | Purpose |
|---------|---------|
| MahApps.Metro | WPF Metro-style theming |
| TagLibSharp | Audio file metadata read/write |
| SixLabors.ImageSharp | Image processing (dominant color extraction for auto-accent) |
| Flurl.Http | HTTP client for external API calls |
| Newtonsoft.Json | JSON serialization |
| FontAwesome5 | Icon fonts in UI |

## Code Conventions
- Always use Flurl, but don't use the `new FlurlRequest`, use '.AppendPathSegments` etc. directly on the URL string.