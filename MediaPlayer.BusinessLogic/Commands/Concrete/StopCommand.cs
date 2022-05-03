using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class StopCommand : IStopCommand
    {
        readonly IState _state;

        public StopCommand(IState state)
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
            _state.SelectMediaItem(_state.GetFirstMediaItemIndex());

            _state.StopMedia();
        }
    }
}
