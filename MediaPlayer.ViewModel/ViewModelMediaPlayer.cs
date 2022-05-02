using MediaPlayer.Model;
using MediaPlayer.ApplicationSettings;
using Generic.PropertyNotify;
using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;

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
        public ISeekbarThumbStartedDraggingCommand SeekbarThumbStartedDraggingCommand { get; set; }
        public ISeekbarThumbCompletedDraggingCommand SeekbarThumbCompletedDraggingCommand { get; set; }
        public ISeekbarPreviewMouseUpCommand SeekbarPreviewMouseUpCommand { get; set; }
        public ITopMostGridDragEnterCommand TopMostGridDragEnterCommand { get; set; }
        public ITopMostGridDropCommand TopMostGridDropCommand { get; set; }
        public ILoadThemeOnWindowLoadedCommand LoadThemeOnWindowLoadedCommand { get; set; }
        public IFocusOnPlayPauseButtonCommand FocusOnPlayPauseButtonCommand { get; set; }

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

            _model = model;
        }
    }
}
