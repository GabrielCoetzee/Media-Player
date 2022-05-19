using Generic.NamedPipes.Wrappers;
using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.EventTriggers.Abstract;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    [Export(CommandNames.MainWindowClosing, typeof(ICommand))]
    public class MainWindowClosingCommand : ICommand
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
