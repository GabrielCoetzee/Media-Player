using Generic.Mediator;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class OpenSettingsWindowCommand : IOpenSettingsWindowCommand
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
            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.OpenApplicationSettings);
        }
    }
}
