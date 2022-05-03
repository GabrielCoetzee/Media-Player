using MediaPlayer.Model;
using MediaPlayer.ApplicationSettings;
using Generic.PropertyNotify;
using MediaPlayer.ViewModel.Commands.Abstract;
using MediaPlayer.ViewModel.Commands.Abstract.EventTriggers;
using System;
using MediaPlayer.Model.Objects.Base;
using MediaPlayer.Model.Collections;
using System.Windows.Controls;
using MediaPlayer.Common.Enumerations;
using System.Windows.Threading;
using System.Linq;

namespace MediaPlayer.ViewModel
{
    public class ViewModelMediaPlayer : PropertyNotifyBase
    {
        private readonly Random _randomIdGenerator = new();

        private MediaItem _selectedMediaItem;
        private MediaItemObservableCollection _mediaItems = new();
        private bool _isLoadingMediaItems;
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

        public bool IsLoadingMediaItems
        {
            get => _isLoadingMediaItems;
            set
            {
                _isLoadingMediaItems = value;
                OnPropertyChanged(nameof(IsLoadingMediaItems));
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
        public ISettingsProvider SettingsProvider { get; set; }
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
        public IFocusOnPlayPauseButtonCommand FocusOnPlayPauseButtonCommand { get; set; }

        public ViewModelMediaPlayer(ISettingsProvider settingsProvider,
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
            IFocusOnPlayPauseButtonCommand focusOnPlayPauseButtonCommand)
        {
            SettingsProvider = settingsProvider;

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
            FocusOnPlayPauseButtonCommand = focusOnPlayPauseButtonCommand;
        }

        public bool IsMediaListEmpty()
        {
            return MediaItems.Count <= 0;
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

        public bool IsPreviousMediaItemAvailable()
        {
            return (!IsMediaListEmpty()) && MediaItems.Any(x => MediaItems.IndexOf(x) == MediaItems.IndexOf(SelectedMediaItem) - 1);
        }
        public bool IsNextMediaItemAvailable()
        {
            return (!IsMediaListEmpty()) && MediaItems.Any(x => MediaItems.IndexOf(x) == MediaItems.IndexOf(SelectedMediaItem) + 1);
        }

        public int GetPreviousMediaItemIndex()
        {
            return MediaItems.IndexOf(SelectedMediaItem) - 1;
        }

        public int GetNextMediaItemIndex()
        {
            return MediaItems.IndexOf(SelectedMediaItem) + 1;
        }

        public int GetFirstMediaItemIndex()
        {
            return MediaItems.IndexOf(MediaItems.First());
        }

        public int GetLastMediaItemIndex()
        {
            return MediaItems.IndexOf(MediaItems.Last());
        }

        public bool IsFirstMediaItemSelected()
        {
            return MediaItems.IndexOf(SelectedMediaItem) == MediaItems.IndexOf(MediaItems.First());
        }

        public bool IsLastMediaItemSelected()
        {
            return MediaItems.IndexOf(SelectedMediaItem) == MediaItems.IndexOf(MediaItems.Last());
        }

        public void SetAccurateCurrentMediaDuration(TimeSpan duration)
        {
            SelectedMediaItem.Duration = duration;
        }

        public bool IsEndOfCurrentMedia(TimeSpan elapsedTime)
        {
            return elapsedTime == SelectedMediaItem.Duration;
        }

        public void PlayPreviousMediaItem()
        {
            if (IsRepeatEnabled && IsFirstMediaItemSelected())
            {
                SelectMediaItem(GetLastMediaItemIndex());
                PlayMedia();

                return;
            }

            SelectMediaItem(GetPreviousMediaItemIndex());
            PlayMedia();
        }

        public void PlayNextMediaItem()
        {
            if (IsRepeatEnabled && IsLastMediaItemSelected())
            {
                SelectMediaItem(GetFirstMediaItemIndex());
                PlayMedia();

                return;
            }

            SelectMediaItem(GetNextMediaItemIndex());

            PlayMedia();
        }

        public void OrderMediaList()
        {
            MediaItems = new MediaItemObservableCollection(MediaItems.OrderBy(x => x.Id));

            IsMediaItemsShuffled = false;
        }

        public void ShuffleMediaList()
        {
            MediaItems = new MediaItemObservableCollection(MediaItems
                .OrderBy(x => x != SelectedMediaItem)
                .ThenBy(x => _randomIdGenerator.Next()));

            IsMediaItemsShuffled = true;
        }
    }
}
