using MediaPlayer.ViewModel.Commands.Abstract.EventTriggers;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete.EventTriggers
{
    public class FocusOnPlayPauseButtonCommand : IFocusOnPlayPauseButtonCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is not Button playPauseButton)
                return;

            playPauseButton.Focus();
        }
    }
}
