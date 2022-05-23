using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.ClearList, typeof(ICommand))]
    public class ClearMediaListCommand : ICommand
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

            return vm.IsMediaListPopulated;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            vm.CurrentPositionTracker.Stop();

            vm.MediaState = MediaState.Stop;
            vm.MediaItems.Clear();

            vm.BusyViewModel.MediaListTitle = string.Empty;
        }
    }
}
