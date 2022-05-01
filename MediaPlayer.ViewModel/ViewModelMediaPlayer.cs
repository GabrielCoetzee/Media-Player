using System.Collections.Generic;
using System.Windows.Input;
using MediaPlayer.Model;
using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.ApplicationSettings;
using Generic.PropertyNotify;
using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.Services.Abstract;

namespace MediaPlayer.ViewModel
{
    public class ViewModelMediaPlayer : PropertyNotifyBase
    {
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
        public ISeekbarValueChangedCommand SeekbarValueChangedCommand { get; set; }
        public ISeekbarThumbStartedDraggingCommand SeekbarThumbStartedDraggingCommand { get; set; }
        public ISeekbarThumbCompletedDraggingCommand SeekbarThumbCompletedDraggingCommand { get; set; }
        public ISeekbarPreviewMouseUpCommand SeekbarPreviewMouseUpCommand { get; set; }

        private readonly IMediaListService _mediaListService;

        private ModelMediaPlayer _model;
        public ModelMediaPlayer Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        public ViewModelMediaPlayer(ModelMediaPlayer model,
            ISettingsProvider settingsProvider,
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
            ISeekbarValueChangedCommand seekbarValueChangedCommand,
            ISeekbarThumbStartedDraggingCommand seekbarThumbStartedDraggingCommand,
            ISeekbarThumbCompletedDraggingCommand seekbarThumbCompletedDraggingCommand,
            IMediaListService mediaListService,
            ISeekbarPreviewMouseUpCommand seekbarPreviewMouseUpCommand)
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
            SeekbarValueChangedCommand = seekbarValueChangedCommand;
            SeekbarThumbStartedDraggingCommand = seekbarThumbStartedDraggingCommand;
            SeekbarThumbCompletedDraggingCommand = seekbarThumbCompletedDraggingCommand;
            SeekbarPreviewMouseUpCommand = seekbarPreviewMouseUpCommand;

            _mediaListService = mediaListService;

            _model = model;
        }

        #region Public Methods

        public void AddToMediaList(IEnumerable<MediaItem> mediaItems)
        {
            this._mediaListService.AddRange(mediaItems);
        }

        public void SetIsLoadingMediaItems(bool isLoadingMediaItems)
        {
            _model.IsLoadingMediaItems = isLoadingMediaItems;
        }

        #endregion
    }
}
