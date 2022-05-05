using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class NextTrackCommand : INextTrackCommand
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

            return vm.IsMediaListPopulated && (vm.IsNextMediaItemAvailable() || vm.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            PlayNextMediaItem(vm);
        }

        private void PlayNextMediaItem(MainViewModel vm)
        {
            if (vm.IsRepeatEnabled && vm.IsLastMediaItemSelected())
            {
                vm.SelectMediaItem(vm.FirstMediaItemIndex());
                vm.PlayMedia();

                return;
            }

            vm.SelectMediaItem(vm.NextMediaItemIndex());
            vm.PlayMedia();
        }
    }
}
