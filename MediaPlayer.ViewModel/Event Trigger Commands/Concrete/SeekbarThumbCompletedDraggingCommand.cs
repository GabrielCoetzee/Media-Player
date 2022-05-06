using System;
using System.Windows.Input;
using MediaPlayer.ViewModel.EventTriggers.Abstract;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    public class SeekbarThumbCompletedDraggingCommand : ISeekbarThumbCompletedDraggingCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return false;

            return vm.IsUserDraggingSeekbarThumb;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            vm.IsUserDraggingSeekbarThumb = false;
        }
    }
}
