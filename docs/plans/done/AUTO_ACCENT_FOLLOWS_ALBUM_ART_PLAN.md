# Auto-Accent Follows Album-Art Changes Plan

## Context

When loading files with metadata-update enabled, `MetadataUpdateService.UpdateMetadataAsync` runs the
album-art fetch (LastFM) and the lyrics fetch (LyricsOVH) in parallel and waits for **both**
(`Task.WhenAll`) before returning. Only after that does `MainViewModel.ProcessFilePathsAsync` send
`MessengerMessages.AutoAdjustAccent`. The album-art task usually finishes first (LyricsOVH frequently
404s / is slow), so the user sees the new cover art on the playing track while the UI accent stays stale
until "Updating Metadata" fully completes.

**Outcome:** raise the accent-recalculation signal as soon as the album-art batch is applied, instead of
waiting for the lyrics phase too.

## Approach

`MetadataUpdateService.UpdateAlbumArtAsync` already has a natural "album art is done" boundary — the
`updateItems.ForEach(x => x.EnrichAlbumArt(...))` line. Send `MessengerMessages.AutoAdjustAccent` right
after it. `MetadataUpdateService` lives in the `MediaPlayer.ViewModel` project, which already references
`Generic.Mediator` and `MediaPlayer.Common.Enumerations`, so this is in-keeping with the layer. The
existing `AutoAdjustAccent` handler in `MessengerRegistrations` reads
`MainViewModel.SelectedMediaItem.AlbumArt` fresh, so no other change is needed.

Rejected alternatives:
- Sending from `AudioItem.DisplayLocalAlbumArt` (the model) — wrong layer (UI side effect in a domain
  entity) and wrong granularity: it runs per-item for every track (builder, batch enrich, corrector), so
  a bulk load would fire N+ messages and re-run dominant-colour extraction N times on the same selected
  track.
- Observing `PropertyChanged` on the selected item from `MainViewModel` — requires manual
  subscribe/unsubscribe lifecycle in the setter.

### Changes — `src/MediaPlayer.ViewModel/Services/Concrete/MetadataUpdateService.cs`

- Add `using Generic.Mediator;` and `using MediaPlayer.Common.Enumerations;`.
- In `UpdateAlbumArtAsync`, after `updateItems.ForEach(x => x.EnrichAlbumArt(...));`, add:
  `Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);`

No changes to `AudioItem`, `MainViewModel`, `ThemeViewModel`, or `MessengerRegistrations`. The existing
`Messenger.Send(AutoAdjustAccent)` at `MainViewModel.cs:149` (after `UpdateMetadataAsync` →
`FixMetadata`) stays as the backstop: it covers the cover.jpg/folder.jpg corrector path and the
"metadata update disabled" path. Net effect: in the common case the accent updates once when album art
lands (mid-"Updating Metadata"), then once more at the end if the corrector changed anything — a
double-update only in the narrow "LastFM gave nothing but a local cover.jpg exists" case.

## Verification

1. `dotnet build src/MediaPlayer.sln`.
2. `dotnet test src/MediaPlayer.ViewModel.Test`.
3. `dotnet run --project src/MediaPlayer.Shell`.
4. Ensure "Update metadata" and "Auto adjust accent" are enabled in settings. Open a folder of MP3s that
   lack embedded cover art (so LastFM supplies art) and where some tracks also lack lyrics (so the lyrics
   phase runs long).
5. Confirm: while "Updating Metadata..." is still showing, when the playing track's cover art appears the
   window accent color changes at (essentially) the same moment — not after the status returns to
   "Media List".
6. Regression checks:
   - Selecting a different track still re-runs auto-accent (uses the new art, or resets if none).
   - Tracks that already have embedded art still get the correct accent on selection.
   - Clearing the media list / closing mid-update doesn't throw.
   - With "Auto adjust accent" off, no accent change occurs.
