using System.Collections.Generic;
using System.Windows.Input;
using MediaPlayer.Model;
using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.ApplicationSettings;
using Generic.PropertyNotify;
using MediaPlayer.BusinessLogic.Commands.Abstract;

namespace MediaPlayer.ViewModel
{
    public class ViewModelMediaPlayer : PropertyNotifyBase
    {
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
            ISeekbarThumbCompletedDraggingCommand seekbarThumbCompletedDraggingCommand)
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

            _model = model;
        }

        #region Public Methods

        public void AddToMediaList(IEnumerable<MediaItem> mediaItems)
        {
            this._model.MediaItems.AddRange(mediaItems);

            if (_model.SelectedMediaItem != null || this._model.IsMediaListEmpty())
                return;

            this._model.SelectMediaItem(this._model.GetFirstMediaItemIndex());
            this._model.PlayMedia();

            this.RefreshUIBindings();
        }

        public void SetIsLoadingMediaItems(bool isLoadingMediaItems)
        {
            _model.IsLoadingMediaItems = isLoadingMediaItems;
        }

        #endregion

        #region Private Methods

        private void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }
}
