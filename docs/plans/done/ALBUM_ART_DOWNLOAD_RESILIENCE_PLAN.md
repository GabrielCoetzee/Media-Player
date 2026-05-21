# Album-Art Download Resilience Plan

## Context

The app terminated with an unhandled `Flurl.Http.FlurlHttpException` thrown from
`LastFmAlbumArtMetadataUpdater.DownloadAlbumArtFromUrlAsync` — once with status **503 (first byte timeout)**
and once with **404 (Not Found)** on `lastfm.freetls.fastly.net/...png` image URLs.

**Why it crashes:** `DownloadAlbumArtFromUrlAsync` calls `url.GetBytesAsync()` with no error handling, and
`MetadataUpdateService.UpdateAlbumArtAsync` only catches cancellation exceptions — so a `FlurlHttpException`
escapes `Parallel.ForEachAsync` → `Task.WhenAll` → `MainViewModel.ProcessFilePathsAsync` → unhandled →
process exit. (The LyricsOVH path already swallows failures at its API layer, which is why lyrics fetch
failures never crash.)

**Why the requests fail at all:** the same image URLs from the exception still serve fine in a browser, so
this is not a stale/dead resource — it's how we make the request:

1. **No `User-Agent` header.** Flurl sends none by default. Last.fm's image CDN (Fastly) rejects UA-less
   requests (404/403/503). A browser always sends a full UA — which is why clicking the link works. Most
   likely cause of the 404.
2. **Request burst — `RuntimeCache.GetOrAddAsync` doesn't dedupe in-flight calls.** It does `TryGetValue`,
   then `await function()`, then stores the result. An album with N tracks shares one art URL, and
   `Parallel.ForEachAsync` fires all N identical downloads simultaneously before any populate the cache. A
   burst of identical GETs is a classic trigger for a Fastly 503.

**Outcome:** fix the root causes (UA header, in-flight dedup), and also make album-art fetch failures
non-fatal with a small retry for transient 503/timeout — so a one-off CDN hiccup degrades to "no art for that
track", never a crash.

## Changes

### 1. `src/Generic/Cache/Concrete/RuntimeCache.cs` — dedupe in-flight requests
Cache the *task* (`ConcurrentDictionary<string, Lazy<Task<T>>>`; the `Lazy` ensures the factory runs exactly
once even under a race). `GetOrAddAsync` returns `await _cache.GetOrAdd(key, _ => new Lazy<Task<T>>(function)).Value`.
Signature of `IRuntimeCache<T>.GetOrAddAsync` unchanged. Only caller is `LastFmAlbumArtMetadataUpdater`.

### 2. `src/MediaPlayer.Model/MediaPlayer.Model.csproj`
Add `Polly.Core` (v8) package reference for `ResiliencePipeline`.

### 3. `src/MediaPlayer.Model/Metadata/Concrete/Updaters/LastFmAlbumArtMetadataUpdater.cs`
- `static readonly ResiliencePipeline` built once: retry on `FlurlHttpException` only when transient
  (`StatusCode is null` → network/timeout, **or** `StatusCode >= 500`, **or** `StatusCode == 408`); do not
  retry 404. `MaxRetryAttempts = 2`, exponential backoff, base delay ~500 ms.
- `DownloadAlbumArtFromUrlAsync`: send a `User-Agent` header, run through the pipeline, wrap in
  `try { ... } catch (FlurlHttpException) { return null; }`.
- `GetAlbumArtAsync` thus returns `null` on any download failure; the existing `albumArt.IsNullOrEmpty()`
  check in `MetadataUpdateService.UpdateAlbumArtAsync` already treats `null` as "no art".

### 4. `src/Integration.LastFM/Services/Concrete/LastFMApi.cs`
Add the same `User-Agent` header to the `track.getinfo` request chain.

## Files

- `src/Generic/Cache/Concrete/RuntimeCache.cs`
- `src/MediaPlayer.Model/MediaPlayer.Model.csproj`
- `src/MediaPlayer.Model/Metadata/Concrete/Updaters/LastFmAlbumArtMetadataUpdater.cs`
- `src/Integration.LastFM/Services/Concrete/LastFMApi.cs`

(No changes to `MetadataUpdateService.cs` or `App.xaml.cs`.)

## Verification

1. `dotnet build src/MediaPlayer.sln` — Polly restores, solution builds.
2. `dotnet test src/MediaPlayer.ViewModel.Test` — no regressions.
3. Manual: `dotnet run --project src/MediaPlayer.Shell`, load a multi-track album of MP3s without embedded art
   ("Update metadata" enabled). App stays up, art populates, and the art URL is requested once not once-per-track.
4. (Optional) Point `DownloadAlbumArtFromUrlAsync` at a URL that 404s and one that 503s; confirm skip-not-crash
   and 2 retry attempts on the 503.
