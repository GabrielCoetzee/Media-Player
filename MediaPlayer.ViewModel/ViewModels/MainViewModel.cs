using Generic.PropertyNotify;
using MediaPlayer.Model.Collections;
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
        private MediaItem _selectedMediaItem;
        private MediaItemObservableCollection _mediaItems = new();

        public readonly DispatcherTimer PositionTracker = new();
        public List<CancellationTokenSource> UpdateMetadataTokenSources = new();

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

        [Import(CommandNames.OpenSettingsWindow)]
        public ICommand OpenSettingsWindowCommand { get; set; }

        [Import(CommandNames.TopMostGridDragEnter)]
        public ICommand TopMostGridDragEnterCommand { get; set; }

        [Import(CommandNames.TopMostGridDrop)]
        public ICommand TopMostGridDropCommand { get; set; }

        [Import(CommandNames.MediaOpened)]
        public ICommand MediaOpenedCommand { get; set; }

        [Import(CommandNames.MainWindowClosing)]
        public ICommand MainWindowClosingCommand { get; set; }

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

        public async Task ProcessFilePathsAsync(IEnumerable<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
                return;

            BusyViewModel.MediaListLoading();

            var mediaItems = await MetadataServices.MetadataReader.ReadFilePathsAsync(filePaths);

            AddMediaItemsToListView(mediaItems);

            BusyViewModel.MediaListPopulated();

            await UpdateMetadataAsync(mediaItems.OfType<AudioItem>());

            Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
        }

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
            UpdateMetadataTokenSources.Add(cts);

            await MetadataServices.MetadataUpdater.UpdateMetadataAsync(audioItems, cts.Token);

            if (UpdateMetadataTokenSources.All(x => x.IsCancellationRequested))
                return;

            BusyViewModel.MediaListPopulated();
        }

        public async Task SaveChangesAsync()
        {
            await ReleaseResourcesAsync();

            if (!SettingsViewModel.SaveMetadataToFile)
                return;

            BusyViewModel.SavingChanges();

            await MetadataServices.MetadataWriter.WriteChangesToFilesInParallel(MediaItems.Where(x => x.IsDirty));
        }

        private async Task ReleaseResourcesAsync()
        {
            await Task.Run(async () =>
            {
                await Parallel.ForEachAsync(UpdateMetadataTokenSources, new CancellationTokenSource().Token, (sources, token) =>
                {
                    sources.Cancel();
                    sources.Dispose();

                    return new ValueTask();
                });
            });

            UpdateMetadataTokenSources.Clear();

            MediaControlsViewModel.SetPlaybackState(MediaState.Stop);

            PositionTracker.Stop();
            SelectedMediaItem = null;
        }

        public void SelectMediaItem(int index) => SelectedMediaItem = MediaItems[index];

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
