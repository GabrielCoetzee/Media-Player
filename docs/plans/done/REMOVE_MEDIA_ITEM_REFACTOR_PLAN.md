# RemoveMediaItem Refactor Plan

This document is the agreed plan for moving `RemoveMediaItem` out of `PlayerShellViewModel` and into `QueueViewModel`, and tightening the "selection mirrors playback" invariant. Implemented on branch `feature/visualizations` alongside other in-flight work.

---

## Context

`PlayerShellViewModel.RemoveMediaItem(MediaItem)` (lines 98–128) currently lives on the shell ViewModel because the operation needs to:

1. Mutate the queue (`QueueViewModel.MediaItems` + `SelectedMediaItem`).
2. Stop playback when the playing item is the only one left (`MediaControlsViewModel.Stop()`).

Since `MediaControlsViewModel` imports `QueueViewModel`, `QueueViewModel` cannot import `MediaControlsViewModel` without creating a circular dependency. The shell was the only place with access to both, so the orchestration landed there.

Findings from grilling the design:

- `MediaControlsViewModel` already subscribes to `QueueViewModel.PropertyChanged` and auto-plays whenever `SelectedMediaItem` changes (lines 158, 185–191). The handler currently calls `Play(item.FilePath?.LocalPath)`, and `Play(null)` is a silent no-op.
- That is why the current `RemoveMediaItem` has to issue `Stop()` *and* `SelectedMediaItem = null` together — the null assignment doesn't stop on its own. Same pattern appears in `ReleaseResources` (lines 130–138).
- There is no place in the codebase that clears `SelectedMediaItem` without also wanting to stop playback. The invariant "engine's current track == `SelectedMediaItem`" is nearly true today; it's just enforced asymmetrically (the Play side is automatic, the Stop side is manual).
- `QueueViewModel` already exposes the helpers needed to express the next-index logic in English: `IsLastMediaItemSelected()`, `GetPreviousMediaItemIndex()`, `GetNextMediaItemIndex()`. No new helper required.
- The command-parameter pattern in this codebase is consistent: commands receive their target VM via `CommandParameter` from XAML; no command resolves a VM via MEF `[Import]`.

---

## Goal

Move `RemoveMediaItem` into `QueueViewModel` where it belongs (it's queue manipulation), and break the implicit dependency on `MediaControlsViewModel` by making the "engine stops when nothing is selected" invariant symmetric — driven entirely through the existing `PropertyChanged` subscription. `PlayerShellViewModel` slims down to shell/panel/window concerns.

---

## Decisions Locked

| # | Decision |
|---|---|
| 1 | **Move `RemoveMediaItem` from `PlayerShellViewModel` to `QueueViewModel`.** Method signature unchanged: `public void RemoveMediaItem(MediaItem item)`. |
| 2 | **Establish "selection mirrors playback" as a true invariant.** `MediaControlsViewModel.QueueViewModel_PropertyChanged` is updated to call `Stop()` when the new `SelectedMediaItem` is null, instead of relying on `Play(null)` being a silent no-op. |
| 3 | **Drop the explicit `Stop()` from `RemoveMediaItem`'s "only item" branch.** Under decision 2, `SelectedMediaItem = null` handles it. |
| 4 | **Drop the explicit `Stop()` from `PlayerShellViewModel.ReleaseResources`.** Same reason — the trailing `SelectedMediaItem = null` already triggers `Stop()` via the PropertyChanged handler. |
| 5 | **Simplify the next-index logic using existing helpers.** Replace the `removedIndex +/- 1` arithmetic with `IsLastMediaItemSelected() ? GetPreviousMediaItemIndex() : GetNextMediaItemIndex()`. No new helper method. |
| 6 | **Command wiring: keep the `CommandParameter`-as-VM convention.** Rename `RemoveMediaItemConverterModel.PlayerShellViewModel` → `QueueViewModel`. Update the XAML `MultiBinding` in `QueuePanel.xaml` to bind to `DataContext.QueueViewModel`. `RemoveMediaItemCommand` calls `model.QueueViewModel.RemoveMediaItem(model.MediaItem)`. |
| 7 | **`RemoveMediaItemCommand` import stays on `PlayerShellViewModel`.** The XAML binds `Command="{Binding DataContext.RemoveMediaItemCommand, ...}"`, where `DataContext` is `PlayerShellViewModel`. Leaving the import there avoids a XAML rebind and matches how other commands (e.g., `ClearMediaListCommand`) are exposed. |
| 8 | **No new event on `QueueViewModel`.** The earlier "raise a `PlayingItemRemoved` event" idea is rejected — the existing `PropertyChanged` plumbing already carries the signal once decision 2 is in place, and adding a parallel event would create two ways to say the same thing. |

---

## Target `QueueViewModel.RemoveMediaItem`

```csharp
public void RemoveMediaItem(MediaItem item)
{
    if (item == null || !MediaItems.Contains(item))
        return;

    var isCurrentlyPlaying = ReferenceEquals(item, SelectedMediaItem);

    if (!isCurrentlyPlaying)
    {
        MediaItems.Remove(item);
        return;
    }

    if (MediaItems.Count == 1)
    {
        SelectedMediaItem = null;
        MediaItems.Remove(item);
        return;
    }

    var nextIndex = IsLastMediaItemSelected()
        ? GetPreviousMediaItemIndex()
        : GetNextMediaItemIndex();

    SelectMediaItem(nextIndex);
    MediaItems.Remove(item);
}
```

## Target `MediaControlsViewModel.QueueViewModel_PropertyChanged`

```csharp
private void QueueViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
{
    if (e.PropertyName != nameof(QueueViewModel.SelectedMediaItem))
        return;

    var item = QueueViewModel.SelectedMediaItem;

    if (item == null)
    {
        Stop();
        return;
    }

    Play(item.FilePath?.LocalPath);
}
```

## Target `PlayerShellViewModel.ReleaseResources`

```csharp
private void ReleaseResources()
{
    QueueViewModel.CancelMediaLoad();
    QueueViewModel.CancelMetadataUpdate();

    QueueViewModel.SelectedMediaItem = null;
}
```

---

## Implementation Steps

1. **Add `RemoveMediaItem` to `QueueViewModel`** in the simplified form above. Place near the other public queue-mutation surface.
2. **Update `MediaControlsViewModel.QueueViewModel_PropertyChanged`** to handle the null-selection case by calling `Stop()`.
3. **Remove `RemoveMediaItem` from `PlayerShellViewModel`.** Drop the now-unused `using MediaPlayer.Model.BusinessEntities.Abstract;` if nothing else needs it (it does — `RemoveMediaItem` is gone but no other type references survive only via it; verify by build).
4. **Simplify `PlayerShellViewModel.ReleaseResources`** by removing the explicit `MediaControlsViewModel?.Stop()` call.
5. **Rename `RemoveMediaItemConverterModel.PlayerShellViewModel` → `QueueViewModel`** (property type changes from `PlayerShellViewModel` to `QueueViewModel`).
6. **Update `RemoveMediaItemMultiValueConverter.Convert`** to populate the renamed property.
7. **Update `RemoveMediaItemCommand`** — `CanExecute` and `Execute` now check `model.QueueViewModel != null` and call `model.QueueViewModel.RemoveMediaItem(model.MediaItem)`.
8. **Update `QueuePanel.xaml` MultiBinding** — change the first `Binding Path="DataContext"` to `Binding Path="DataContext.QueueViewModel"`.
9. **Build the solution** (`dotnet build src/MediaPlayer.sln`).
10. **Run the test suite** (`dotnet test src/MediaPlayer.ViewModel.Test`).
11. **Manual smoke test** — remove a non-playing track, remove the currently-playing track with others queued, remove the last remaining track. Verify playback transitions match expectations.

---

## Files Touched

| File | Change |
|---|---|
| `MediaPlayer.ViewModel/ViewModels/QueueViewModel.cs` | Add `RemoveMediaItem` method. |
| `MediaPlayer.ViewModel/ViewModels/MediaControlsViewModel.cs` | Update `QueueViewModel_PropertyChanged` to call `Stop()` on null selection. |
| `MediaPlayer.ViewModel/ViewModels/PlayerShellViewModel.cs` | Remove `RemoveMediaItem`; simplify `ReleaseResources`. |
| `MediaPlayer.ViewModel/Converter Objects/RemoveMediaItemConverterModel.cs` | Rename `PlayerShellViewModel` property → `QueueViewModel` (and change its type). |
| `MediaPlayer.View/Converters/MultiValueConverters/RemoveMediaItemMultiValueConverter.cs` | Update populated property. |
| `MediaPlayer.ViewModel/Commands/Concrete/RemoveMediaItemCommand.cs` | Update `CanExecute` / `Execute` to route through `model.QueueViewModel`. |
| `MediaPlayer.View/Components/QueuePanel.xaml` | Update MultiBinding to pass `DataContext.QueueViewModel`. |

---

## Risks & Edge Cases

- **`Play(null)` becoming `Stop()` is a behavior change for any *other* code path that sets `SelectedMediaItem = null`.** Only two writers exist today (`PlayerShellViewModel.RemoveMediaItem` and `ReleaseResources`), and both *want* the stop. Confirmed via grep before locking decision 2.
- **Selection-driven Stop on app shutdown.** `ReleaseResources` runs from `SaveChangesAsync` (which itself runs from `MainWindowClosing`). Issuing `Stop()` via the PropertyChanged handler at shutdown is fine — same call as before, just routed differently.
- **MEF subscription lifecycle.** No new subscription is introduced. The existing `OnImportsSatisfied` already wires `QueueViewModel.PropertyChanged` — we only change the handler body.
- **Item with non-null reference but null `FilePath`.** `Play(item.FilePath?.LocalPath)` returns early; engine does not stop. Same as today. Not introduced or worsened by this change.
