using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.Escape, typeof(ICommand))]
    public class EscapeCommand : ICommand
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

            return vm.IsSettingsOpen || vm.IsLyricsOpen;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            if (vm.IsSettingsOpen)
            {
                vm.IsSettingsOpen = false;
                return;
            }

            if (vm.IsLyricsOpen)
                vm.IsLyricsOpen = false;
        }
    }
}
