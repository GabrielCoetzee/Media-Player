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

        public readonly DispatcherTimer CurrentPositionTracker = new();
        public List<CancellationTokenSource> UpdateMetadataTokenSources = new();

        public MediaItem SelectedMediaItem
        {
            get => _selectedMediaItem;
            set
            {
                _selectedMediaItem = value;
                OnPropertyChanged(nameof(SelectedMediaItem));
                OnPropertyChanged(nameof(IsMediaListPopulated));
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
        public bool IsMediaListPopulated => MediaItems.Count >= 1;

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
        public IMetadataReaderService MetadataReaderService { get; set; }

        [Import]
        public IMetadataUpdateService MetadataUpdateService { get; set; }

        [Import]
        public IMetadataWriterService MetadataWriterService { get; set; }

        public MainViewModel()
        {
            MEF.Container?.SatisfyImportsOnce(this);
        }

        public async Task ProcessDroppedContentAsync(IEnumerable<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
                return;

            BusyViewModel.IsLoading = true;
            BusyViewModel.MediaListTitle = "Media List Loading...";

            var mediaItems = await MetadataReaderService.ReadFilePathsAsync(filePaths);

            AddMediaItemsToListView(mediaItems);

            if (SettingsManager.IsUpdateMetadataEnabled)
            {
                BusyViewModel.MediaListTitle = "Updating Metadata...";

                var cts = new CancellationTokenSource();
                UpdateMetadataTokenSources.Add(cts);

                await MetadataUpdateService.UpdateMetadataAsync(mediaItems.OfType<AudioItem>(), cts.Token);

                if (UpdateMetadataTokenSources.All(x => x.IsCancellationRequested))
                    return;
            }

            BusyViewModel.IsLoading = false;
            BusyViewModel.MediaListTitle = "Media List";
        }

        private void AddMediaItemsToListView(IEnumerable<MediaItem> mediaItems)
        {
            MediaItems.AddRange(mediaItems);

            if (SelectedMediaItem != null)
                return;

            SelectMediaItem(FirstMediaItemIndex());
            MediaControlsViewModel.PlayMedia();

            CommandManager.InvalidateRequerySuggested();
        }

        public async Task SaveChangesAsync()
        {
            await Task.Run(() => UpdateMetadataTokenSources.ForEach(x => x.Cancel()));
            UpdateMetadataTokenSources.Clear();

            MediaControlsViewModel.StopMedia();

            CurrentPositionTracker.Stop();
            SelectedMediaItem = null;

            if (!SettingsManager.IsSaveMetadataToFileEnabled)
                return;

            BusyViewModel.IsLoading = true;
            BusyViewModel.MediaListTitle = "Saving Changes...";

            await MetadataWriterService.WriteChangesToFilesInParallel(MediaItems.Where(x => x.IsDirty));
        }

        public void SelectMediaItem(int index)
        {
            SelectedMediaItem = MediaItems[index];
        }

        public bool IsPreviousMediaItemAvailable() => (IsMediaListPopulated) && PreviousMediaItemIndex() >= FirstMediaItemIndex();

        public bool IsNextMediaItemAvailable() => (IsMediaListPopulated) && NextMediaItemIndex() <= LastMediaItemIndex();

        public int PreviousMediaItemIndex() => MediaItems.IndexOf(SelectedMediaItem) - 1;

        public int NextMediaItemIndex() => MediaItems.IndexOf(SelectedMediaItem) + 1;

        public int FirstMediaItemIndex() => MediaItems.IndexOf(MediaItems.First());

        public int LastMediaItemIndex() => MediaItems.IndexOf(MediaItems.Last());

        public bool IsFirstMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == FirstMediaItemIndex();

        public bool IsLastMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == LastMediaItemIndex();

        public bool IsEndOfCurrentlyPlayingMedia() => SelectedMediaItem.ElapsedTime == SelectedMediaItem.Duration;
    }
}
