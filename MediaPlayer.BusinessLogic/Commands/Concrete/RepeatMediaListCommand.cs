using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class RepeatMediaListCommand : IRepeatMediaListCommand
    {
        readonly IState _state;

        public RepeatMediaListCommand(IState state)
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
            return !_state.IsMediaListEmpty() && _state.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            _state.IsRepeatEnabled = !_state.IsRepeatEnabled;
        }
    }
}
