# Lyrics Retrieval Speedup Plan — bound the latency

## Context

Loading audio files with metadata-update enabled feels slow, and the lyrics phase (LyricsOVH) is the laggy
part. Causes:

1. **`api.lyrics.ovh` is slow/flaky and 404s a lot** — unavoidable floor; not addressed here.
2. **No per-request timeout** — `LyricsOvhApi.GetLyricsAsync` called `GetJsonAsync<>()` with no timeout, so
   it inherited Flurl's **100-second** default. A single stuck request ties up a worker for that long.
3. **Concurrency capped at CPU core count** — `MetadataUpdateService.UpdateLyricsAsync` (and
   `UpdateAlbumArtAsync`) used `Parallel.ForEachAsync` with no `MaxDegreeOfParallelism`, so it defaulted to
   `Environment.ProcessorCount` (~4–8). For network-bound work that's far too low; combined with #2 a few
   slow tracks at the front stall everything behind them.

Caching is deliberately *not* part of this: lyrics are keyed by artist+title (unique per track), so unlike
the album-art path (many tracks share one image URL) there's nothing to dedupe within a load.

**Outcome:** a hung lyrics request fails fast (~5s) instead of after 100s, and many more tracks are fetched
concurrently — so the lyrics phase no longer stalls on a handful of slow requests.

## Changes

### 1. `src/Integration.LyricsOVH/Services/Concrete/LyricsOvhApi.cs` — per-request timeout

Added `const int TimeoutSeconds = 5;` and `.WithTimeout(TimeoutSeconds)` before `.GetJsonAsync<>()`. The
existing `catch (Exception)` already catches `FlurlHttpTimeoutException` and returns `null`, so a timeout
is treated exactly like "no lyrics found" (same as a 404) — no behavior change beyond the bound. 5s is
generous for a working response and tunable via the const.

### 2. `src/MediaPlayer.ViewModel/Services/Concrete/MetadataUpdateService.cs` — raise parallelism

Added `const int MaxConcurrentRequests = 8;`. In **both** `UpdateLyricsAsync` and `UpdateAlbumArtAsync`,
the `Parallel.ForEachAsync` calls now pass a
`ParallelOptions { MaxDegreeOfParallelism = MaxConcurrentRequests, CancellationToken = token }` instead of
the bare token. 8 is a deliberate middle ground — well above low-end core counts, not so high it risks 429s
from a fragile free API; tunable via the const.

While here, the "which items still need fetching" filtering moved up into `UpdateMetadataAsync`
(`audioItems.Where(x => !x.HasAlbumArt)` / `!x.HasLyrics`), so the two private methods no longer carry
duplicate `if (audioItem.HasX) return;` guards; each materializes its incoming sequence once
(`var updateItems = audioItems.ToList();`) and uses that list for both the parallel loop and the enrich
pass. `Task.Run(..., token)` and the cancellation catches are unchanged.

Out of scope: LastFM's `GetTrackInfoAsync` also has no timeout — a candidate for the same treatment later.

## Verification

1. `dotnet build src/MediaPlayer.sln`.
2. `dotnet test src/MediaPlayer.ViewModel.Test`.
3. `dotnet run --project src/MediaPlayer.Shell` with "Update metadata" enabled; open a folder of ~20+ MP3s
   without embedded lyrics (a mix of mainstream tracks that have lyrics and obscure/instrumental ones that
   404). Confirm the "Updating Metadata..." phase finishes noticeably faster and never hangs for ~100s on
   one bad track.
4. Confirm lyrics still populate for tracks that have them (open the lyrics pane on a known track).
5. Clear the media list mid-update — confirm it still cancels cleanly without throwing.
