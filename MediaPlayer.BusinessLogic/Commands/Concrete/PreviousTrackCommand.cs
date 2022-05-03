using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class PreviousTrackCommand : IPreviousTrackCommand
    {
        readonly IState _state;

        public PreviousTrackCommand(IState state)
        {
            _state = state;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return !_state.IsMediaListEmpty() && (_state.IsPreviousMediaItemAvailable() || _state.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            _state.PlayPreviousMediaItem();
        }
    }
}
