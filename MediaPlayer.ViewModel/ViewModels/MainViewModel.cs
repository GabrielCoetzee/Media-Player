using Generic.PropertyNotify;
using System;
using MediaPlayer.Model.Collections;
using System.Windows.Controls;
using MediaPlayer.Common.Enumerations;
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
using MediaPlayer.ViewModel.Commands.Concrete;
using MediaPlayer.ViewModel.Commands.Abstract;
using Generic.DependencyInjection;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Threading;
using System.Windows;

namespace MediaPlayer.ViewModel
{
    [Export]
    public class MainViewModel : PropertyNotifyBase
    {
        private MediaItem _selectedMediaItem;
        private MediaItemObservableCollection _mediaItems = new();
        private TimeSpan _mediaElementPosition;
        private MediaState _mediaState = MediaState.Pause;
        private VolumeLevel _mediaVolume = VolumeLevel.Full;
        private bool _isUserDraggingSeekbarThumb;
        private bool _isRepeatEnabled;
        private bool _isMediaItemsShuffled;

        public readonly DispatcherTimer CurrentPositionTracker = new();
        public List<CancellationTokenSource> UpdateMetadataTokenSources = new List<CancellationTokenSource>();

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
        public bool IsMediaListPopulated => MediaItems.Count >= 1;

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

        public TimeSpan MediaElementPosition
        {
            get => _mediaElementPosition;
            set
            {
                _mediaElementPosition = value;

                OnPropertyChanged(nameof(MediaElementPosition));
            }
        }

        public MediaState MediaState
        {
            get => _mediaState;
            set
            {
                _mediaState = value;
                OnPropertyChanged(nameof(MediaState));
            }
        }

        public VolumeLevel MediaVolume
        {
            get => _mediaVolume;
            set
            {
                _mediaVolume = value;
                OnPropertyChanged(nameof(MediaVolume));
            }
        }

        public bool IsUserDraggingSeekbarThumb
        {
            get => _isUserDraggingSeekbarThumb;
            set
            {
                _isUserDraggingSeekbarThumb = value;
                OnPropertyChanged(nameof(IsUserDraggingSeekbarThumb));
            }
        }

        public bool IsRepeatEnabled
        {
            get => _isRepeatEnabled;
            set
            {
                _isRepeatEnabled = value;
                OnPropertyChanged(nameof(IsRepeatEnabled));
            }
        }

        public bool IsMediaItemsShuffled
        {
            get => _isMediaItemsShuffled;
            set
            {
                _isMediaItemsShuffled = value;
                OnPropertyChanged(nameof(IsMediaItemsShuffled));
            }
        }

        [Import(CommandNames.Shuffle)]
        public ICommand ShuffleCommand { get; set; }

        [Import(CommandNames.OpenSettingsWindow)]
        public ICommand OpenSettingsWindowCommand { get; set; }

        [Import(CommandNames.PlayPause)]
        public ICommand PlayPauseCommand { get; set; }

        [Import(CommandNames.Mute)]
        public ICommand MuteCommand { get; set; }

        [Import(CommandNames.PreviousTrack)]
        public ICommand PreviousTrackCommand { get; set; }

        [Import(CommandNames.Stop)]
        public ICommand StopCommand { get; set; }

        [Import(CommandNames.Repeat)]
        public ICommand RepeatMediaListCommand { get; set; }

        [Import(CommandNames.ClearList)]
        public ICommand ClearMediaListCommand { get; set; }

        [Import(CommandNames.StartedDragging)]
        public ICommand SeekbarThumbStartedDraggingCommand { get; set; }

        [Import(CommandNames.CompletedDragging)]
        public ICommand SeekbarThumbCompletedDraggingCommand { get; set; }

        [Import(CommandNames.TopMostGridDragEnter)]
        public ICommand TopMostGridDragEnterCommand { get; set; }

        [Import(CommandNames.TopMostGridDrop)]
        public ICommand TopMostGridDropCommand { get; set; }

        [Import(CommandNames.NextTrack)]
        public ICommand NextTrackCommand { get; set; }

        [Import(CommandNames.MediaOpened)]
        public ICommand MediaOpenedCommand { get; set; }

        [Import(CommandNames.AddMedia)]
        public ICommand AddMediaCommand { get; set; }

        [Import(CommandNames.LoadThemeOnWindowLoaded)]
        public ICommand LoadThemeOnWindowLoadedCommand { get; set; }

        [Import(CommandNames.MainWindowClosing)]
        public ICommand MainWindowClosingCommand { get; set; }

        [Import]
        public ISeekbarPreviewMouseUpCommand SeekbarPreviewMouseUpCommand { get; set; }

        [Import]
        public ISettingsManager SettingsManager { get; set; }

        [Import]
        public BusyViewModel BusyViewModel { get; set; }

        [Import]
        public IMetadataReaderService MetadataReaderService { get; set; }

        [Import]
        public IMetadataUpdateService MetadataUpdateService { get; set; }

        [Import]
        public IMetadataWriterService MetadataWriterService { get; set; }

        public MainViewModel()
        {
            MEF.Container?.SatisfyImportsOnce(this);

            SeekbarPreviewMouseUpCommand.ChangeMediaPosition += SeekbarPreviewMouseUpCommand_ChangeMediaPosition;
        }

        private void SeekbarPreviewMouseUpCommand_ChangeMediaPosition(object sender, SliderPositionEventArgs e)
        {
            MediaElementPosition = TimeSpan.FromSeconds(e.Position);
        }

        public async Task ProcessDroppedContentAsync(IEnumerable<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
                return;

            BusyViewModel.IsLoading = true;
            BusyViewModel.MediaListTitle = "Media List Loading...";

            var mediaItems = await MetadataReaderService.ReadFilePathsAsync(filePaths);

            AddToListView(mediaItems);

            BusyViewModel.MediaListTitle = "Updating Metadata...";

            var cts = new CancellationTokenSource();
            UpdateMetadataTokenSources.Add(cts);

            await MetadataUpdateService.UpdateMetadataAsync(mediaItems.OfType<AudioItem>(), cts.Token);

            if (UpdateMetadataTokenSources.All(x => x.IsCancellationRequested))
                return;

            BusyViewModel.IsLoading = false;
            BusyViewModel.MediaListTitle = "Media List";
        }

        private void AddToListView(IEnumerable<MediaItem> mediaItems)
        {
            MediaItems.AddRange(mediaItems);

            if (SelectedMediaItem != null)
                return;

            SelectMediaItem(FirstMediaItemIndex());
            PlayMedia();
        }

        public async Task SaveChangesAsync(bool shutdownApplication)
        {
            UpdateMetadataTokenSources.ForEach(x => x.Cancel());
            UpdateMetadataTokenSources.Clear();

            StopMedia();

            CurrentPositionTracker.Stop();
            SelectedMediaItem = null;

            BusyViewModel.IsLoading = true;
            BusyViewModel.MediaListTitle = "Saving Changes...";

            await MetadataWriterService.WriteChangesToFilesInParallel(MediaItems.Where(x => x.IsDirty));

            if (!shutdownApplication)
            {
                MediaItems.Clear();
                MediaItems = new MediaItemObservableCollection();

                BusyViewModel.MediaListTitle = string.Empty;
                BusyViewModel.IsLoading = false;

                return;
            }

            Application.Current.Shutdown(0);
        }

        public void PlayMedia()
        {
            MediaState = MediaState.Play;
        }

        public void PauseMedia()
        {
            MediaState = MediaState.Pause;
        }

        public void StopMedia()
        {
            MediaState = MediaState.Stop;
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
