using ControlzEx.Theming;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.Theming;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
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
