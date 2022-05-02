using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
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
            return !_model.IsMediaListEmpty() && _model.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaElement ui_mediaElement)
                return;

            PollMediaPosition(ui_mediaElement);
        }

        private void PollMediaPosition(MediaElement mediaElement)
        {
            _model.SetAccurateCurrentMediaDuration(mediaElement.NaturalDuration.TimeSpan);

            _model.CurrentPositionTracker.Tick += (sender, args) => TrackMediaPosition(mediaElement);

            _model.CurrentPositionTracker.Start();
        }

        private void TrackMediaPosition(MediaElement mediaElement)
        {
            if (!_model.IsUserDraggingSeekbarThumb)
                _model.SelectedMediaItem.ElapsedTime = mediaElement.Position;

            if (!_model.IsEndOfCurrentMedia(_model.SelectedMediaItem.ElapsedTime))
                return;

            if (_nextTrackCommand.CanExecute(null))
                _nextTrackCommand.Execute(null);

            RefreshUIBindings();
        }

        private static void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
