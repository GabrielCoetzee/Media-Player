using Generic.Wrappers;
using MediaPlayer.ViewModel.EventTriggers.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    public class MainWindowClosingCommand : IMainWindowClosingCommand
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
            var pipeManager = new NamedPipeManager("MediaPlayer");
            pipeManager.StopServer();
        }
    }
}
