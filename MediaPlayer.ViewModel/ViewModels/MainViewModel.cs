using MediaPlayer.ApplicationSettings;
using Generic.PropertyNotify;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using MediaPlayer.Model.Collections;
using System.Windows.Controls;
using MediaPlayer.Common.Enumerations;
using System.Windows.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Windows.Input;
using MediaPlayer.ViewModel.EventTriggers.Concrete;
using MediaPlayer.ViewModel.EventTriggers.Abstract;
using MediaPlayer.ViewModel.ViewModels;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete;
using Generic.Mediator;

namespace MediaPlayer.ViewModel
{
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

        public ISettingsProviderViewModel SettingsProviderViewModel { get; set; }
        public MetadataReaderResolver MetadataReaderResolver { get; set; }
        public IOpenSettingsWindowCommand OpenSettingsWindowCommand { get; set; }
        public IShuffleCommand ShuffleCommand { get; set; }
        public IAddMediaCommand AddMediaCommand { get; set; }
        public IPlayPauseCommand PlayPauseCommand { get; set; }
        public IMuteCommand MuteCommand { get; set; }
        public IPreviousTrackCommand PreviousTrackCommand { get; set; }
        public IStopCommand StopCommand { get; set; }
        public INextTrackCommand NextTrackCommand { get; set; }
        public IRepeatMediaListCommand RepeatMediaListCommand { get; set; }
        public IMediaOpenedCommand MediaOpenedCommand { get; set; }
        public IClearMediaListCommand ClearMediaListCommand { get; set; }
        public ISeekbarThumbStartedDraggingCommand SeekbarThumbStartedDraggingCommand { get; set; }
        public ISeekbarThumbCompletedDraggingCommand SeekbarThumbCompletedDraggingCommand { get; set; }
        public ISeekbarPreviewMouseUpCommand SeekbarPreviewMouseUpCommand { get; set; }
        public ITopMostGridDragEnterCommand TopMostGridDragEnterCommand { get; set; }
        public ITopMostGridDropCommand TopMostGridDropCommand { get; set; }
        public ILoadThemeOnWindowLoadedCommand LoadThemeOnWindowLoadedCommand { get; set; }
        public BusyViewModel BusyViewModel { get; set; }

        public MainViewModel(ISettingsProviderViewModel settingsProviderViewModel,
            MetadataReaderResolver metadataReaderResolver,
            IOpenSettingsWindowCommand openSettingsWindowCommand,
            IShuffleCommand shuffleCommand,
            IAddMediaCommand addMediaCommand,
            IPlayPauseCommand playPauseCommand,
            IMuteCommand muteCommand,
            IPreviousTrackCommand previousTrackCommand,
            IStopCommand stopCommand,
            INextTrackCommand nextTrackCommand,
            IRepeatMediaListCommand repeatMediaListCommand,
            IMediaOpenedCommand mediaOpenedCommand,
            IClearMediaListCommand clearMediaListCommand,
            ISeekbarThumbStartedDraggingCommand seekbarThumbStartedDraggingCommand,
            ISeekbarThumbCompletedDraggingCommand seekbarThumbCompletedDraggingCommand,
            ISeekbarPreviewMouseUpCommand seekbarPreviewMouseUpCommand,
            ITopMostGridDragEnterCommand topMostGridDragEnterCommand,
            ITopMostGridDropCommand topMostGridDropCommand,
            ILoadThemeOnWindowLoadedCommand loadThemeOnWindowLoadedCommand,
            BusyViewModel busyViewModel)
        {
            SettingsProviderViewModel = settingsProviderViewModel;
            MetadataReaderResolver = metadataReaderResolver;

            OpenSettingsWindowCommand = openSettingsWindowCommand;
            ShuffleCommand = shuffleCommand;
            AddMediaCommand = addMediaCommand;
            PlayPauseCommand = playPauseCommand;
            MuteCommand = muteCommand;
            PreviousTrackCommand = previousTrackCommand;
            StopCommand = stopCommand;
            NextTrackCommand = nextTrackCommand;
            RepeatMediaListCommand = repeatMediaListCommand;
            MediaOpenedCommand = mediaOpenedCommand;
            ClearMediaListCommand = clearMediaListCommand;
            SeekbarThumbStartedDraggingCommand = seekbarThumbStartedDraggingCommand;
            SeekbarThumbCompletedDraggingCommand = seekbarThumbCompletedDraggingCommand;
            SeekbarPreviewMouseUpCommand = seekbarPreviewMouseUpCommand;
            TopMostGridDragEnterCommand = topMostGridDragEnterCommand;
            TopMostGridDropCommand = topMostGridDropCommand;
            LoadThemeOnWindowLoadedCommand = loadThemeOnWindowLoadedCommand;

            BusyViewModel = busyViewModel;

            SeekbarPreviewMouseUpCommand.ChangeMediaPosition += SeekbarPreviewMouseUpCommand_ChangeMediaPosition;

            Messenger<MessengerMessages>.Register(MessengerMessages.ProcessContent, async (args) =>
            {
                await ProcessDroppedContentAsync(args as IEnumerable<string>);

                CommandManager.InvalidateRequerySuggested();
            });
        }

        public async Task ProcessDroppedContentAsync(IEnumerable<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
                return;

            BusyViewModel.IsLoadingMediaItems = true;

            var mediaItems = await ReadMediaItemsAsync(filePaths);

            AddMediaItems(mediaItems);

            BusyViewModel.IsLoadingMediaItems = false;
        }

        private void SeekbarPreviewMouseUpCommand_ChangeMediaPosition(object sender, SliderPositionEventArgs e)
        {
            MediaElementPosition = TimeSpan.FromSeconds(e.Position);
        }

        private async Task<IEnumerable<MediaItem>> ReadMediaItemsAsync(IEnumerable filePaths)
        {
            var supportedFiles = new List<MediaItem>();

            await Task.Run(() =>
            {
                var metadataReader = MetadataReaderResolver.Resolve(MetadataReaders.Taglib);
                var supportedFileFormats = SettingsProviderViewModel.SupportedFileFormats;

                foreach (var path in filePaths)
                {
                    var isFolder = Directory.Exists(path.ToString());

                    if (isFolder)
                    {
                        var mediaItems = Directory
                            .EnumerateFiles(path.ToString(), "*.*", SearchOption.AllDirectories)
                            .Where(file => supportedFileFormats.Any(file.ToLower().EndsWith))
                            .Select((x) => metadataReader.GetFileMetadata(x));

                        supportedFiles.AddRange(mediaItems);
                    }
                    else
                    {
                        if (supportedFileFormats.Any(x => x.ToLower() == Path.GetExtension(path.ToString().ToLower())))
                        {
                            var mediaItem = metadataReader.GetFileMetadata(path.ToString());

                            supportedFiles.Add(mediaItem);
                        }
                    }

                }
            });

            return supportedFiles;
        }

        public void AddMediaItems(IEnumerable<MediaItem> mediaItems)
        {
            MediaItems.AddRange(mediaItems);

            if (SelectedMediaItem != null)
            {
                BusyViewModel.IsLoadingMediaItems = false;
                return;
            }

            SelectMediaItem(FirstMediaItemIndex());
            PlayMedia();
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

        public bool IsPreviousMediaItemAvailable() => (IsMediaListPopulated) && MediaItems.Any(x => MediaItems.IndexOf(x) == MediaItems.IndexOf(SelectedMediaItem) - 1);

        public bool IsNextMediaItemAvailable() => (IsMediaListPopulated) && MediaItems.Any(x => MediaItems.IndexOf(x) == MediaItems.IndexOf(SelectedMediaItem) + 1);

        public int PreviousMediaItemIndex() => MediaItems.IndexOf(SelectedMediaItem) - 1;

        public int NextMediaItemIndex() => MediaItems.IndexOf(SelectedMediaItem) + 1;

        public int FirstMediaItemIndex() => MediaItems.IndexOf(MediaItems.First());

        public int LastMediaItemIndex() => MediaItems.IndexOf(MediaItems.Last());

        public bool IsFirstMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == MediaItems.IndexOf(MediaItems.First());

        public bool IsLastMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == MediaItems.IndexOf(MediaItems.Last());

        public bool IsEndOfCurrentMedia() => SelectedMediaItem.ElapsedTime == SelectedMediaItem.Duration;
    }
}
