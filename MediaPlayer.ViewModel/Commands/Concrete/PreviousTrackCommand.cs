using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class PreviousTrackCommand : IPreviousTrackCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return false;

            return !vm.IsMediaListEmpty() && (vm.IsPreviousMediaItemAvailable() || vm.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return;

            PlayPreviousMediaItem(vm);
        }

        private void PlayPreviousMediaItem(ViewModelMediaPlayer vm)
        {
            if (vm.IsRepeatEnabled && vm.IsFirstMediaItemSelected())
            {
                vm.SelectMediaItem(vm.LastMediaItemIndex());
                vm.PlayMedia();

                return;
            }

            vm.SelectMediaItem(vm.PreviousMediaItemIndex());
            vm.PlayMedia();
        }
    }
}
