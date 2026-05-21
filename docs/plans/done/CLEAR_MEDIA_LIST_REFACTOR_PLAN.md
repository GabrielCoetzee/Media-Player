# Clear Media List Refactor Plan

## Context

`ClearMediaListCommand` currently targets `PlayerShellViewModel` and calls `PlayerShellViewModel.SaveChangesAsync()`, which bundles three responsibilities under a name that only describes one:

1. **Release** — cancel in-flight `AddMediaAsync` / `UpdateMetadataAsync`, null the selection (which now stops playback via the "selection mirrors playback" invariant).
2. **Persist** — write dirty metadata to disk, gated on `SettingsViewModel.SaveMetadataToFile`.
3. **Notify** — set the `BusyViewModel.SavingChanges` banner.

The name `SaveChangesAsync` hides the release side-effects, including the playback stop. Two call sites (`ClearMediaListCommand` and the `SaveChangesToDirtyFiles` messenger on shutdown) both get the bundled behavior, even though shutdown doesn't care about the selection-clear.

Every piece of state the bundled operation touches (`MediaItems`, `MetadataServices`, `SettingsViewModel.SaveMetadataToFile`, `BusyViewModel`) is already owned by or imported into `QueueViewModel`. The "shell-level" feel was coming from the bundled name, not from the underlying work.

## Goal

- Split `SaveChangesAsync` into named, single-purpose operations.
- Move the clear-list ceremony onto `QueueViewModel`, where its state lives.
- Retarget `ClearMediaListCommand` to `QueueViewModel` (mirror the `StopCommand` pattern: `[Import]` on the owning VM, XAML binds via `QueueViewModel.ClearMediaListCommand`).
- Inline the shutdown handler's three steps at the messenger registration — no facade.
- Remove dead `PlayerShellViewModel.SaveChangesAsync` and `ReleaseResources`.

## Decisions Locked

1. **`SaveDirtyMetadataAsync` lives on `QueueViewModel`.** Every dependency it has (MediaItems, MetadataServices, SettingsViewModel, BusyViewModel) is already on QueueViewModel.
2. **`ClearMediaListAsync` is one coarse method.** It encapsulates cancel → null selection → save → clear → reset busy banner. The queue owns getting the order right, not the command.
3. **`BusyViewModel.InitialStartupState()` is set inside `ClearMediaListAsync`.** Consistent with every other busy transition (set inside the queue operation that triggers it).
4. **Shutdown handler resolves `QueueViewModel` directly and inlines the three steps.** No new abstraction; the call site is the only place that pairs cancel + save without a clear.
5. **`ClearMediaListCommand` `[Import]` moves to `QueueViewModel`.** Mirrors the `StopCommand` pattern (command imported on the VM whose state it operates on). XAML reaches it via `QueueViewModel.ClearMediaListCommand`.
6. **`CommandParameter` is `{Binding QueueViewModel}`** — single binding, no MultiBinding needed (no compound parameter required).
7. **`PlayerShellViewModel.SaveChangesAsync` and `ReleaseResources` are deleted.** Both become unreferenced. The `[Import(CommandNames.ClearList)]` on `PlayerShellViewModel` is also removed.
8. **Cancel-then-save ordering is preserved.** The original code cancels in-flight async before reading dirty items; we keep that order inside `ClearMediaListAsync` and at the inlined shutdown call site. (A pre-existing race between background `WriteChangesToFilesInParallel` iteration and UI-thread `AddRange` exists; not introduced or worsened by this refactor, and out of scope here.)

## Target Code

### `QueueViewModel.cs` — add

```csharp
[Import(CommandNames.ClearList)]
public ICommand ClearMediaListCommand { get; set; }

public async Task SaveDirtyMetadataAsync()
{
    if (!SettingsViewModel.SaveMetadataToFile)
        return;

    BusyViewModel.SavingChanges();

    await MetadataServices.MetadataWriter
        .WriteChangesToFilesInParallel(MediaItems.Where(x => x.IsDirty));
}

public async Task ClearMediaListAsync()
{
    CancelMediaLoad();
    CancelMetadataUpdate();
    SelectedMediaItem = null;
    await SaveDirtyMetadataAsync();
    MediaItems.Clear();
    BusyViewModel.InitialStartupState();
}
```

### `ClearMediaListCommand.cs` — rewrite

```csharp
public bool CanExecute(object parameter)
    => parameter is QueueViewModel vm && vm.IsMediaListPopulated;

public async void Execute(object parameter)
{
    if (parameter is not QueueViewModel vm)
        return;

    await vm.ClearMediaListAsync();
}
```

### `PlayerShellViewModel.cs` — remove

```csharp
// Delete:
[Import(CommandNames.ClearList)]
public ICommand ClearMediaListCommand { get; set; }

public async Task SaveChangesAsync() { ... }
private void ReleaseResources() { ... }
```

### `MessengerRegistrations.cs` — inline

```csharp
public static void SaveChangesToDirtyFiles(CompositionContainer container)
{
    Messenger<MessengerMessages, ShutdownArgs>.Register(MessengerMessages.SaveChangesToDirtyFiles, async (args) =>
    {
        var queue = container?.GetExportedValue<QueueViewModel>();

        queue.CancelMediaLoad();
        queue.CancelMetadataUpdate();
        await queue.SaveDirtyMetadataAsync();

        if (args.IsEnabled)
            Application.Current.Shutdown(0);
    });
}
```

### `QueuePanel.xaml` — rebind

```xml
<MenuItem Header="Clear all"
          Command="{Binding QueueViewModel.ClearMediaListCommand}"
          CommandParameter="{Binding QueueViewModel}" />
```

## Implementation Steps

1. Add `SaveDirtyMetadataAsync` and `ClearMediaListAsync` to `QueueViewModel`.
2. Move the `[Import(CommandNames.ClearList)]` from `PlayerShellViewModel` to `QueueViewModel`.
3. Rewrite `ClearMediaListCommand.CanExecute` and `Execute` to target `QueueViewModel`.
4. Update `QueuePanel.xaml` `<MenuItem Header="Clear all" ...>` binding to use `QueueViewModel.ClearMediaListCommand` and `{Binding QueueViewModel}`.
5. Inline the cancel + save in `MessengerRegistrations.SaveChangesToDirtyFiles` — resolve `QueueViewModel` instead of `PlayerShellViewModel`.
6. Delete `PlayerShellViewModel.SaveChangesAsync` and `ReleaseResources`.
7. Build the solution and run the test suite. Both should pass with no new failures (no tests reference the removed code).
8. Manual smoke test:
   - Add a few tracks → click Clear all → list empties, busy banner returns to startup state, playback stops.
   - Add tracks → toggle `SaveMetadataToFile` off → click Clear all → list empties, no save attempted.
   - Add tracks while another load is in flight → click Clear all → first load cancels cleanly.
   - Close the window with a dirty item → save still runs on shutdown.

## Files Touched

- `MediaPlayer.ViewModel/ViewModels/QueueViewModel.cs` — add `SaveDirtyMetadataAsync`, `ClearMediaListAsync`, `ClearMediaListCommand` [Import].
- `MediaPlayer.ViewModel/ViewModels/PlayerShellViewModel.cs` — remove `SaveChangesAsync`, `ReleaseResources`, `ClearMediaListCommand` [Import].
- `MediaPlayer.ViewModel/Commands/Concrete/ClearMediaListCommand.cs` — retarget to `QueueViewModel`.
- `MediaPlayer.Shell/Messenger Registrations/MessengerRegistrations.cs` — inline cancel + save in the shutdown handler.
- `MediaPlayer.View/Components/QueuePanel.xaml` — rebind `MenuItem` to `QueueViewModel.ClearMediaListCommand` and `QueueViewModel` parameter.

## Risks & Edge Cases

- **Pre-existing race** between background-thread `WriteChangesToFilesInParallel` and UI-thread `AddRange` is preserved unchanged. Not addressing in this refactor.
- **Re-entrant clear** (button still enabled while save is awaiting) is theoretical and pre-existing. Second invocation would re-issue cancellations (no-op on already-cancelled tokens) and trigger a second save with fewer dirty items. Not addressing in this refactor.
- **Shutdown handler now depends on `QueueViewModel` directly via MEF.** `QueueViewModel` is already MEF-exported and reachable from the container; no new wiring needed.
- **No test fallout.** `MediaPlayer.ViewModel.Test` has no references to `SaveChangesAsync`, `ReleaseResources`, or `ClearMediaListCommand`.
