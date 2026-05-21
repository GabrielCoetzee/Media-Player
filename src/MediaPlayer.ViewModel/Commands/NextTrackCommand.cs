using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands
{
    [Export(CommandNames.NextTrack, typeof(ICommand))]
    public class NextTrackCommand : ICommand
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

            return vm.QueueViewModel.IsMediaListPopulated
                && (vm.QueueViewModel.IsNextMediaItemAvailable() || vm.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return;

            PlayNextMediaItem(vm);
        }

        private void PlayNextMediaItem(MediaControlsViewModel vm)
        {
            var index = vm.QueueViewModel.GetNextMediaItemIndex();

            if (vm.IsRepeatEnabled && vm.QueueViewModel.IsLastMediaItemSelected())
                index = vm.QueueViewModel.GetFirstMediaItemIndex();

            vm.QueueViewModel.SelectMediaItem(index);
            vm.MediaState = MediaState.Play;
        }
    }
}
