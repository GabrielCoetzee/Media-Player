using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class MediaOpenedCommand : IMediaOpenedCommand
    {
        readonly ModelMediaPlayer _model;
        readonly INextTrackCommand _nextTrackCommand;

        public MediaOpenedCommand(ModelMediaPlayer model, INextTrackCommand nextTrackCommand)
        {
            _model = model;
            _nextTrackCommand = nextTrackCommand;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return !this._model.IsMediaListEmpty() && _model.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaElement ui_mediaElement)
                return;

            PollMediaPosition(ui_mediaElement);
        }

        private void PollMediaPosition(MediaElement mediaElement)
        {
            this._model.SetAccurateCurrentMediaDuration(mediaElement.NaturalDuration.TimeSpan);

            this._model.MediaPositionTracker.Tick += (sender, args) => TrackMediaPosition(mediaElement);

            this._model.MediaPositionTracker.Start();
        }

        private void TrackMediaPosition(MediaElement mediaElement)
        {
            if (!_model.IsUserDraggingSeekbarThumb)
                this._model.MediaPosition = mediaElement.Position;

            if (!this._model.IsEndOfCurrentMedia(this._model.SelectedMediaItem.ElapsedTime))
                return;

            if (_nextTrackCommand.CanExecute(null))
            {
                _nextTrackCommand.Execute(null);
            }

            RefreshUIBindings();
        }

        private static void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
