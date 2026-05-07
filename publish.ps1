param(
    [Parameter(Mandatory)]
    [string]$Version
)

$ErrorActionPreference = 'Stop'

$repoRoot    = $PSScriptRoot
$publishDir  = Join-Path $repoRoot 'publish'
$releasesDir = Join-Path $repoRoot 'releases'
$shellProj   = Join-Path $repoRoot 'src\MediaPlayer.Shell\MediaPlayer.Shell.csproj'

if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }

dotnet tool restore
if ($LASTEXITCODE -ne 0) { throw "dotnet tool restore failed" }

dotnet publish $shellProj `
    -c Release `
    -r win-x64 `
    --self-contained `
    -o $publishDir
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

dotnet vpk pack `
    --packId MediaPlayer `
    --packVersion $Version `
    --packDir $publishDir `
    --mainExe MediaPlayer.Shell.exe `
    --packTitle 'Media Player' `
    --packAuthors 'Andre Gabriel Coetzee' `
    --outputDir $releasesDir
if ($LASTEXITCODE -ne 0) { throw "vpk pack failed" }

Write-Host ""
Write-Host "Installer: $releasesDir\MediaPlayerSetup.exe" -ForegroundColor Green
