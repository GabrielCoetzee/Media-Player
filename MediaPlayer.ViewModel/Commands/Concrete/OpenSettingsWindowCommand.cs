using Generic.Mediator;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.OpenSettingsWindow, typeof(ICommand))]
    public class OpenSettingsWindowCommand : ICommand
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
            Messenger<MessengerMessages>.Send(MessengerMessages.OpenApplicationSettings);
        }
    }
}
