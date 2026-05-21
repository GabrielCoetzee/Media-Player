using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands
{
    [Export(CommandNames.Stop, typeof(ICommand))]
    public class StopCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return false;

            return vm.QueueViewModel.IsMediaListPopulated;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return;

            vm.QueueViewModel.SelectMediaItem(vm.QueueViewModel.GetFirstMediaItemIndex());
            vm.MediaState = MediaState.Stop;
        }
    }
}
