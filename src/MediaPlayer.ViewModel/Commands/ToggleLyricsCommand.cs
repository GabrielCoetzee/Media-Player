using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.ViewModel.Commands
{
    [Export(CommandNames.ToggleLyrics, typeof(ICommand))]
    public class ToggleLyricsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is not PlayerShellViewModel vm)
                return;

            vm.IsLyricsOpen = !vm.IsLyricsOpen;
        }
    }
}
