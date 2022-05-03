using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.BusinessLogic.State.Abstract;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
{
    public class SeekbarThumbStartedDraggingCommand : ISeekbarThumbStartedDraggingCommand
    {
        readonly IState _state;

        public SeekbarThumbStartedDraggingCommand(IState state)
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
            return _state.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            _state.IsUserDraggingSeekbarThumb = true;
        }
    }
}
