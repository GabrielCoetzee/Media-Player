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
using MediaPlayer.Settings;
using MediaPlayer.Common.Constants;
using Generic.DependencyInjection;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Threading;
using System.Collections.Specialized;

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
        public ISettingsManager SettingsManager { get; set; }

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

            await AddMediaItemsToListViewAsync(filePaths);
            await UpdateMetadataAsync(MediaItems.OfType<AudioItem>());
        }

        private async Task AddMediaItemsToListViewAsync(IEnumerable<string> filePaths)
        {
            BusyViewModel.MediaListLoading();

            var mediaItems = await MetadataServices.MetadataReader.ReadFilePathsAsync(filePaths);

            MediaItems.AddRange(mediaItems);

            if (SelectedMediaItem != null)
                return;

            SelectMediaItem(FirstMediaItem());
            MediaControlsViewModel.PlayMedia();

            //CommandManager.InvalidateRequerySuggested();

            BusyViewModel.MediaListPopulated();
        }

        private async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems)
        {
            if (!SettingsManager.UpdateMetadata || !audioItems.Any())
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

            if (!SettingsManager.SaveMetadataToFile)
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

            MediaControlsViewModel.StopMedia();

            PositionTracker.Stop();
            SelectedMediaItem = null;
        }

        public void SelectMediaItem(int index) => SelectedMediaItem = MediaItems[index];

        public bool IsPreviousMediaItemAvailable() => (IsMediaListPopulated) && PreviousMediaItem() >= FirstMediaItem();

        public bool IsNextMediaItemAvailable() => (IsMediaListPopulated) && NextMediaItem() <= LastMediaItem();

        public int PreviousMediaItem() => MediaItems.IndexOf(SelectedMediaItem) - 1;

        public int NextMediaItem() => MediaItems.IndexOf(SelectedMediaItem) + 1;

        public int FirstMediaItem() => MediaItems.IndexOf(MediaItems.First());

        public int LastMediaItem() => MediaItems.IndexOf(MediaItems.Last());

        public bool IsFirstMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == FirstMediaItem();

        public bool IsLastMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == LastMediaItem();

        public bool IsEndOfCurrentlyPlayingMedia() => SelectedMediaItem.ElapsedTime == SelectedMediaItem.Duration;
    }
}
