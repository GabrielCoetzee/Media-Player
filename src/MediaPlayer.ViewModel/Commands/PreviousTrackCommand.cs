using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands
{
    [Export(CommandNames.PreviousTrack, typeof(ICommand))]
    public class PreviousTrackCommand : ICommand
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

            return vm.QueueViewModel.IsMediaListPopulated && (vm.QueueViewModel.IsPreviousMediaItemAvailable() || vm.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return;

            PlayPreviousMediaItem(vm);
        }

        private void PlayPreviousMediaItem(MediaControlsViewModel vm)
        {
            var index = vm.QueueViewModel.GetPreviousMediaItemIndex();

            if (vm.IsRepeatEnabled && vm.QueueViewModel.IsFirstMediaItemSelected())
                index = vm.QueueViewModel.GetLastMediaItemIndex();

            vm.QueueViewModel.SelectMediaItem(index);
            vm.MediaState = MediaState.Play;
        }
    }
}
