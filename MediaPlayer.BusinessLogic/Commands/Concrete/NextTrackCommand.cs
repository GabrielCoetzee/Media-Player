using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class NextTrackCommand : INextTrackCommand
    {
        readonly IState _state;

        public NextTrackCommand(IState state)
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
            return !_state.IsMediaListEmpty() && (_state.IsNextMediaItemAvailable() || _state.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            _state.PlayNextMediaItem();
        }
    }
}
