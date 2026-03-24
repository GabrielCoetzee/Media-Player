# Upgrade MediaPlayer.sln from .NET 7 to .NET 10

## Context

The solution currently targets **net7.0 / net7.0-windows**, which reached end-of-life in May 2024. .NET 10 is the current LTS release. This upgrade updates the target framework across all 10 projects, bumps all NuGet packages to .NET 10-compatible versions, addresses breaking API changes in 3 dependencies (Flurl, ImageSharp, NUnit), and removes obsolete package references.

---

## Task 1: Prerequisites & Directory.Build.props

Create `Directory.Build.props` at the solution root to centralize the target framework.

**Create** `Directory.Build.props`:
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
  </PropertyGroup>
</Project>
```

**Then remove** `<TargetFramework>` from these 9 csproj files (they inherit `net10.0-windows`):
- `Generic\Generic.csproj`
- `Integration.LastFM\Integration.LastFM.csproj`
- `Integration.LyricsOVH\Integration.LyricsOVH.csproj`
- `MediaPlayer.Settings\MediaPlayer.Settings.csproj`
- `MediaPlayer.Model\MediaPlayer.Model.csproj`
- `MediaPlayer.ViewModel\MediaPlayer.ViewModel.csproj`
- `MediaPlayer.View\MediaPlayer.View.csproj`
- `MediaPlayer.Shell\MediaPlayer.Shell.csproj`
- `MediaPlayer.ViewModel.Test\MediaPlayer.ViewModel.Test.csproj`

**Override** in `MediaPlayer.Common\MediaPlayer.Common.csproj` -- set `<TargetFramework>net10.0</TargetFramework>` (this project is platform-agnostic, no Windows dependency).

**Verify:** `dotnet restore MediaPlayer.sln`

---

## Task 2: Upgrade Generic project

**File:** `Generic\Generic.csproj`

Update packages:
| Package | From | To |
|---|---|---|
| Microsoft.Extensions.Configuration | 7.0.0 | 10.0.0 |
| Microsoft.Extensions.DependencyInjection | 7.0.0 | 10.0.0 |
| Microsoft.Extensions.Hosting | 7.0.1 | 10.0.0 |
| Microsoft.Extensions.Options | 7.0.1 | 10.0.0 |
| Newtonsoft.Json | 13.0.2 | 13.0.3 |
| System.ComponentModel.Composition | 7.0.0 | 10.0.0 |

No code changes needed.

**Verify:** `dotnet build Generic\Generic.csproj`

---

## Task 3: Upgrade Integration.LastFM (Flurl 3 -> 4 breaking changes)

**File:** `Integration.LastFM\Integration.LastFM.csproj`

Package changes:
| Package | Action |
|---|---|
| Flurl 3.0.7 | **REMOVE** (merged into Flurl.Http 4.x) |
| Flurl.Http 3.2.4 | Update to **4.0.2** |
| Newtonsoft.Json 13.0.2 | **REMOVE** (Flurl 4.x uses System.Text.Json) |
| System.ComponentModel.Composition 7.0.0 | Update to **10.0.0** |

**Code changes required:**

1. `Integration.LastFM\Contracts\LastFmResponseModel.cs` (line 1, 69):
   - Replace `using Newtonsoft.Json;` with `using System.Text.Json.Serialization;`
   - Replace `[JsonProperty("#text")]` with `[JsonPropertyName("#text")]`
   - The existing `using System.Text.Json;` on line 2 can be removed (only the `.Serialization` sub-namespace is needed)

2. `Integration.LastFM\Services\Concrete\LastFMApi.cs` (line 1):
   - Remove `using Flurl;` (the `AppendPathSegments` extension is now in `Flurl.Http`)

**Verify:** `dotnet build Integration.LastFM\Integration.LastFM.csproj`

---

## Task 4: Upgrade Integration.LyricsOVH (Flurl 3 -> 4)

**File:** `Integration.LyricsOVH\Integration.LyricsOVH.csproj`

Package changes:
| Package | Action |
|---|---|
| Flurl 3.0.7 | **REMOVE** |
| Flurl.Http 3.2.4 | Update to **4.0.2** |
| System.ComponentModel.Composition 7.0.0 | Update to **10.0.0** |

**Code change:**
- `Integration.LyricsOVH\Services\Concrete\LyricsOvhApi.cs` (line 1): Remove `using Flurl;`

**Verify:** `dotnet build Integration.LyricsOVH\Integration.LyricsOVH.csproj`

---

## Task 5: Upgrade MediaPlayer.Settings (ImageSharp 2 -> 3 breaking changes)

**File:** `MediaPlayer.Settings\MediaPlayer.Settings.csproj`

Update packages:
| Package | From | To |
|---|---|---|
| ControlzEx | 5.0.2 | 5.0.2 (no update) |
| MahApps.Metro | 2.4.9 | 2.4.10 |
| Microsoft.Extensions.Configuration.Abstractions | 7.0.0 | 10.0.0 |
| Microsoft.Extensions.Options | 7.0.1 | 10.0.0 |
| SixLabors.ImageSharp | 2.1.3 | 3.1.6 |

**Code change required** -- `MediaPlayer.Settings\Services\Concrete\ImageSharpColorService.cs`:

The `image[x, y]` pixel indexer was removed in ImageSharp 3.x. Replace the nested for-loop (lines 30-42) with the `ProcessPixelRows` API:

```csharp
image.ProcessPixelRows(accessor =>
{
    for (int y = 0; y < accessor.Height; y++)
    {
        Span<Rgba32> row = accessor.GetRowSpan(y);
        for (int x = 0; x < row.Length; x++)
        {
            var pixel = row[x];
            r += Convert.ToInt32(pixel.R);
            g += Convert.ToInt32(pixel.G);
            b += Convert.ToInt32(pixel.B);
            totalPixels++;
        }
    }
});
```

Add `using System;` if not already present (needed for `Span<T>`).

**Verify:** `dotnet build MediaPlayer.Settings\MediaPlayer.Settings.csproj`

---

## Task 6: Upgrade MediaPlayer.Model

**File:** `MediaPlayer.Model\MediaPlayer.Model.csproj`

Update packages:
| Package | From | To |
|---|---|---|
| System.Drawing.Common | 7.0.0 | 10.0.0 |
| TagLibSharp | 2.3.0 | 2.3.0 (no update) |

No code changes needed.

**Verify:** `dotnet build MediaPlayer.Model\MediaPlayer.Model.csproj`

---

## Task 7: Upgrade MediaPlayer.ViewModel

**File:** `MediaPlayer.ViewModel\MediaPlayer.ViewModel.csproj`

Package changes:
| Package | Action |
|---|---|
| Microsoft.CSharp 4.7.0 | **REMOVE** (built into runtime) |
| System.Data.DataSetExtensions 4.5.0 | **REMOVE** (unused) |
| System.ComponentModel.TypeConverter 4.3.0 | **REMOVE** (built into runtime) |
| Microsoft.Extensions.Caching.Abstractions 7.0.0 | Update to **10.0.0** |
| System.ComponentModel.Composition 7.0.0 | Update to **10.0.0** |

No code changes needed.

**Verify:** `dotnet build MediaPlayer.ViewModel\MediaPlayer.ViewModel.csproj`

---

## Task 8: Upgrade MediaPlayer.View

**File:** `MediaPlayer.View\MediaPlayer.View.csproj`

Package changes:
| Package | Action |
|---|---|
| Microsoft.CSharp 4.7.0 | **REMOVE** |
| System.Data.DataSetExtensions 4.5.0 | **REMOVE** |
| MahApps.Metro 2.4.9 | Update to **2.4.10** |
| System.ComponentModel.Composition 7.0.0 | Update to **10.0.0** |
| ControlzEx 5.0.2 | No change |
| FontAwesome5 2.1.11 | No change |

Also remove legacy ClickOnce / bootstrapper properties (lines 5-19 and 107-117 in the csproj) -- these are .NET Framework era artifacts with no effect.

No code changes needed.

**Verify:** `dotnet build MediaPlayer.View\MediaPlayer.View.csproj`

---

## Task 9: Upgrade MediaPlayer.Shell

**File:** `MediaPlayer.Shell\MediaPlayer.Shell.csproj`

Package changes:
| Package | Action |
|---|---|
| Microsoft.CSharp 4.7.0 | **REMOVE** |
| System.Data.DataSetExtensions 4.5.0 | **REMOVE** |
| Microsoft.Extensions.Configuration 7.0.0 | Update to **10.0.0** |
| Microsoft.Extensions.Configuration.Abstractions 7.0.0 | Update to **10.0.0** |
| Microsoft.Extensions.Configuration.FileExtensions 7.0.0 | Update to **10.0.0** |
| Microsoft.Extensions.Configuration.Json 7.0.0 | Update to **10.0.0** |
| Microsoft.Extensions.DependencyInjection 7.0.0 | Update to **10.0.0** |
| Microsoft.Extensions.Options.ConfigurationExtensions 7.0.0 | Update to **10.0.0** |
| ControlzEx 5.0.2 | No change |
| FontAwesome5 2.1.11 | No change |
| MahApps.Metro 2.4.9 | Update to **2.4.10** |

No code changes needed.

**Verify:** `dotnet build MediaPlayer.Shell\MediaPlayer.Shell.csproj`

---

## Task 10: Upgrade MediaPlayer.ViewModel.Test

**File:** `MediaPlayer.ViewModel.Test\MediaPlayer.ViewModel.Test.csproj`

Update packages:
| Package | From | To |
|---|---|---|
| Microsoft.NET.Test.Sdk | 17.3.2 | 17.12.0 |
| Moq | 4.18.4 | 4.20.72 |
| NUnit | 3.13.3 | 4.3.2 |
| NUnit3TestAdapter | 4.2.1 | 4.6.0 |
| NUnit.Analyzers | 3.3.0 | 4.6.0 |
| coverlet.collector | 3.1.2 | 6.0.4 |

No code changes needed -- existing tests already use the NUnit 4-compatible `Assert.That` constraint model.

**Verify:** `dotnet test MediaPlayer.ViewModel.Test\MediaPlayer.ViewModel.Test.csproj`

---

## Task 11: Full solution build & test

1. Clean old build artifacts: `dotnet clean MediaPlayer.sln` then delete `bin/` and `obj/` folders
2. Full rebuild: `dotnet build MediaPlayer.sln`
3. Run tests: `dotnet test MediaPlayer.sln`
4. Launch application and verify: window renders, theme loads, media playback works

---

## Summary

**Packages updated:** 21 | **Packages removed:** 7 | **Packages unchanged:** 3
**Source files requiring code changes:** 4 files (across Tasks 3, 4, 5)
**New files created:** 1 (`Directory.Build.props`)
