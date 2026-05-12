using Generic.PropertyNotify;
using MediaPlayer.Model.Collections;
using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;
using MediaPlayer.ViewModel.ViewModels;
using MediaPlayer.Model.BusinessEntities.Abstract;
using System.ComponentModel.Composition;
using MediaPlayer.Common.Constants;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Threading;
using System.Collections.Specialized;
using MediaPlayer.Settings.ViewModels;
using Generic.Mediator;
using MediaPlayer.Common.Enumerations;
using System.Windows.Controls;

namespace MediaPlayer.ViewModel
{
    [Export]
    public class MainViewModel : NotifyPropertyChanged
    {
        private const int FlushBatchSize = 25;
        private const int FlushIntervalMs = 150;

        private MediaItem _selectedMediaItem;
        private MediaItemObservableCollection _mediaItems = new();
        private bool _isLyricsOpen;
        private bool _isQueueOpen = true;
        private bool _isSettingsOpen;
        private readonly List<CancellationTokenSource> _updateMetadataTokenSources = new();
        private readonly List<CancellationTokenSource> _loadMediaTokenSources = new();

        public readonly DispatcherTimer PositionTracker = new();

        public MediaItem SelectedMediaItem
        {
            get => _selectedMediaItem;
            set
            {
                _selectedMediaItem = value;
                OnPropertyChanged(nameof(SelectedMediaItem));

                Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
            }
        }

        public MediaItemObservableCollection MediaItems
        {
            get => _mediaItems;
            set
            {
                _mediaItems = value;
                OnPropertyChanged(nameof(MediaItems));
            }
        }
        public bool IsMediaListPopulated => MediaItems.Count > 0;

        public bool IsLyricsOpen
        {
            get => _isLyricsOpen;
            set
            {
                _isLyricsOpen = value;
                OnPropertyChanged(nameof(IsLyricsOpen));
            }
        }

        public bool IsQueueOpen
        {
            get => _isQueueOpen;
            set
            {
                _isQueueOpen = value;
                OnPropertyChanged(nameof(IsQueueOpen));
            }
        }

        public bool IsSettingsOpen
        {
            get => _isSettingsOpen;
            set
            {
                _isSettingsOpen = value;
                OnPropertyChanged(nameof(IsSettingsOpen));
            }
        }

        [Import(CommandNames.TopMostGridDragEnter)]
        public ICommand TopMostGridDragEnterCommand { get; set; }

        [Import(CommandNames.TopMostGridDrop)]
        public ICommand TopMostGridDropCommand { get; set; }

        [Import(CommandNames.MediaOpened)]
        public ICommand MediaOpenedCommand { get; set; }

        [Import(CommandNames.MainWindowClosing)]
        public ICommand MainWindowClosingCommand { get; set; }

        [Import(CommandNames.ToggleLyrics)]
        public ICommand ToggleLyricsCommand { get; set; }

        [Import(CommandNames.ToggleQueue)]
        public ICommand ToggleQueueCommand { get; set; }

        [Import(CommandNames.Escape)]
        public ICommand EscapeCommand { get; set; }

        [Import(CommandNames.RemoveMediaItem)]
        public ICommand RemoveMediaItemCommand { get; set; }

        [Import]
        public SettingsViewModel SettingsViewModel { get; set; }

        [Import]
        public BusyViewModel BusyViewModel { get; set; }

        [Import]
        public MediaControlsViewModel MediaControlsViewModel { get; set; }

        [Import]
        public IMetadataServices MetadataServices { get; set; }

        public MainViewModel()
        {
            MediaItems.CollectionChanged += MediaItems_CollectionChanged;
        }

        private void MediaItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsMediaListPopulated));
        }

        public async Task AddMediaAsync(IEnumerable<string> paths)
        {
            if (paths == null || !paths.Any())
                return;

            var cts = new CancellationTokenSource();
            _loadMediaTokenSources.Add(cts);

            BusyViewModel.MediaListLoading();

            var newlyAddedItems = new List<MediaItem>();
            var pendingItems = new List<MediaItem>();
            var sinceLastFlush = Stopwatch.StartNew();

            try
            {
                await foreach (var mediaItem in MetadataServices.MetadataReader.EnumerateMediaItemsAsync(paths, cts.Token))
                {
                    newlyAddedItems.Add(mediaItem);
                    pendingItems.Add(mediaItem);

                    if (pendingItems.Count < FlushBatchSize && sinceLastFlush.ElapsedMilliseconds < FlushIntervalMs)
                        continue;

                    AddMediaItemsToListView(pendingItems);
                    pendingItems.Clear();
                    sinceLastFlush.Restart();
                }

                if (pendingItems.Count > 0)
                    AddMediaItemsToListView(pendingItems);
            }
            catch (OperationCanceledException)
            {
                // Media list was cleared (or the app is shutting down) — stop adding the rest of this batch.
                return;
            }
            finally
            {
                _loadMediaTokenSources.Remove(cts);
                cts.Dispose();
            }

            BusyViewModel.MediaListPopulated();

            await UpdateMetadataAsync(newlyAddedItems.OfType<AudioItem>());

            Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
        }

        public void CancelMediaLoad() => _loadMediaTokenSources.ForEach(x => x.Cancel());

        private void AddMediaItemsToListView(IEnumerable<MediaItem> mediaItems)
        {
            MediaItems.AddRange(mediaItems);

            if (SelectedMediaItem != null)
                return;

            SelectMediaItem(GetFirstMediaItemIndex());
            MediaControlsViewModel.SetPlaybackState(MediaState.Play);

            CommandManager.InvalidateRequerySuggested();
        }

        private async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems)
        {
            if (!SettingsViewModel.UpdateMetadata || !audioItems.Any())
                return;

            BusyViewModel.UpdatingMetadata();

            var cts = new CancellationTokenSource();
            _updateMetadataTokenSources.Add(cts);

            try
            {
                await MetadataServices.MetadataUpdater.UpdateMetadataAsync(audioItems, cts.Token);

                if (cts.IsCancellationRequested)
                    return;

                BusyViewModel.MediaListPopulated();

                MetadataServices.MetadataCorrector.FixMetadata(audioItems.OfType<AudioItem>());
            }
            finally
            {
                _updateMetadataTokenSources.Remove(cts);
                cts.Dispose();
            }
        }

        public void CancelMetadataUpdate() => _updateMetadataTokenSources.ForEach(x => x.Cancel());

        public async Task SaveChangesAsync()
        {
            ReleaseResources();

            if (!SettingsViewModel.SaveMetadataToFile)
                return;

            BusyViewModel.SavingChanges();

            await MetadataServices.MetadataWriter.WriteChangesToFilesInParallel(MediaItems.Where(x => x.IsDirty));
        }

        private void ReleaseResources()
        {
            CancelMediaLoad();
            CancelMetadataUpdate();

            MediaControlsViewModel.SetPlaybackState(MediaState.Stop);

            PositionTracker.Stop();
            SelectedMediaItem = null;
        }

        public void SelectMediaItem(int index) => SelectedMediaItem = MediaItems[index];

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
                MediaControlsViewModel.SetPlaybackState(MediaState.Stop);
                PositionTracker.Stop();
                SelectedMediaItem = null;
                MediaItems.Remove(item);
                return;
            }

            var removedIndex = MediaItems.IndexOf(item);
            var nextIndex = removedIndex < MediaItems.Count - 1 ? removedIndex + 1 : removedIndex - 1;

            SelectMediaItem(nextIndex);
            MediaControlsViewModel.SetPlaybackState(MediaState.Play);
            MediaItems.Remove(item);
        }

        public bool IsPreviousMediaItemAvailable() => IsMediaListPopulated && GetPreviousMediaItemIndex() >= GetFirstMediaItemIndex();

        public bool IsNextMediaItemAvailable() => IsMediaListPopulated && GetNextMediaItemIndex() <= GetLastMediaItemIndex();

        public int GetPreviousMediaItemIndex() => MediaItems.IndexOf(SelectedMediaItem) - 1;

        public int GetNextMediaItemIndex() => MediaItems.IndexOf(SelectedMediaItem) + 1;

        public int GetFirstMediaItemIndex() => MediaItems.IndexOf(MediaItems.First());

        public int GetLastMediaItemIndex() => MediaItems.IndexOf(MediaItems.Last());

        public bool IsFirstMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == GetFirstMediaItemIndex();

        public bool IsLastMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == GetLastMediaItemIndex();

        public bool IsEndOfCurrentlyPlayingMedia() => SelectedMediaItem.ElapsedTime == SelectedMediaItem.Duration;
    }
}
