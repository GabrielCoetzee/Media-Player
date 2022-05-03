using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.BusinessLogic.State.Abstract;
using MediaPlayer.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
{
    public class MediaOpenedCommand : IMediaOpenedCommand
    {
        readonly IState _state;
        readonly INextTrackCommand _nextTrackCommand;

        public MediaOpenedCommand(IState state, INextTrackCommand nextTrackCommand)
        {
            _state = state;
            _nextTrackCommand = nextTrackCommand;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return !_state.IsMediaListEmpty() && _state.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaElement ui_mediaElement)
                return;

            PollMediaPosition(ui_mediaElement);
        }

        private void PollMediaPosition(MediaElement mediaElement)
        {
            _state.SetAccurateCurrentMediaDuration(mediaElement.NaturalDuration.TimeSpan);

            _state.CurrentPositionTracker.Tick += (sender, args) => TrackMediaPosition(mediaElement);

            _state.CurrentPositionTracker.Start();
        }

        private void TrackMediaPosition(MediaElement mediaElement)
        {
            if (!_state.IsUserDraggingSeekbarThumb)
                _state.SelectedMediaItem.ElapsedTime = mediaElement.Position;

            if (!_state.IsEndOfCurrentMedia(_state.SelectedMediaItem.ElapsedTime))
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
