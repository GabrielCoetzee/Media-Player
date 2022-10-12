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

namespace MediaPlayer.ViewModel
{
    [Export]
    public class MainViewModel : PropertyNotifyBase
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
                OnPropertyChanged(nameof(IsMediaListPopulated));
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

        [Import(CommandNames.LoadThemeOnWindowLoaded)]
        public ICommand LoadThemeOnWindowLoadedCommand { get; set; }

        [Import(CommandNames.MainWindowClosing)]
        public ICommand MainWindowClosingCommand { get; set; }

        [Import]
        public ISettingsManager SettingsManager { get; set; }

        [Import]
        public BusyViewModel BusyViewModel { get; set; }

        [Import]
        public MediaControlsViewModel MediaControlsViewModel { get; set; }

        [Import]
        public IMetadataAggregator MetadataAggregator { get; set; }

        public MainViewModel()
        {
            MEF.Container?.SatisfyImportsOnce(this);
        }

        public async Task ProcessFilePathsAsync(IEnumerable<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
                return;

            BusyViewModel.MediaListLoading();

            var mediaItems = await MetadataAggregator.MetadataReader.ReadFilePathsAsync(filePaths);

            AddMediaItemsToListView(mediaItems);

            BusyViewModel.MediaListFinishedLoading();

            await UpdateMetadataAsync(mediaItems.OfType<AudioItem>());
        }

        private async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems)
        {
            if (!SettingsManager.IsUpdateMetadataEnabled || !audioItems.Any())
                return;

            BusyViewModel.UpdatingMetadata();

            var cts = new CancellationTokenSource();
            UpdateMetadataTokenSources.Add(cts);

            await MetadataAggregator.MetadataUpdater.UpdateMetadataAsync(audioItems, cts.Token);

            if (UpdateMetadataTokenSources.All(x => x.IsCancellationRequested))
                return;

            BusyViewModel.MediaListFinishedLoading();
        }

        private void AddMediaItemsToListView(IEnumerable<MediaItem> mediaItems)
        {
            MediaItems.AddRange(mediaItems);
            OnPropertyChanged(nameof(IsMediaListPopulated));

            if (SelectedMediaItem != null)
                return;

            SelectMediaItem(FirstMediaItem());
            MediaControlsViewModel.PlayMedia();

            CommandManager.InvalidateRequerySuggested();
        }

        public async Task SaveChangesAsync()
        {
            await ReleaseResourcesAsync();

            if (!SettingsManager.IsSaveMetadataToFileEnabled)
                return;

            BusyViewModel.SavingChanges();

            await MetadataAggregator.MetadataWriter.WriteChangesToFilesInParallel(MediaItems.Where(x => x.IsDirty));
        }

        private async Task ReleaseResourcesAsync()
        {
            await Task.Run(() => UpdateMetadataTokenSources.ForEach(x => x.Cancel()));
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
