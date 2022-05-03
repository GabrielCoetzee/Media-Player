using MediaPlayer.ViewModel.Commands.Abstract.EventTriggers;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete.EventTriggers
{
    public class SeekbarThumbCompletedDraggingCommand : ISeekbarThumbCompletedDraggingCommand
    {
        public SeekbarThumbCompletedDraggingCommand()
        {
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return false;

            return vm.IsUserDraggingSeekbarThumb;
        }

        public void Execute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return;

            vm.IsUserDraggingSeekbarThumb = false;
        }
    }
}
