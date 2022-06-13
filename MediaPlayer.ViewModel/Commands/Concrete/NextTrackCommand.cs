using MediaPlayer.Common.Constants;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
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
            if (parameter is not MainViewModel vm)
                return false;

            return vm.IsMediaListPopulated && (vm.IsNextMediaItemAvailable() || vm.MediaControlsViewModel.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            PlayNextMediaItem(vm);
        }

        private void PlayNextMediaItem(MainViewModel vm)
        {
            var index = vm.NextMediaItemIndex();

            if (vm.MediaControlsViewModel.IsRepeatEnabled && vm.IsLastMediaItemSelected())
                index = vm.FirstMediaItemIndex();

            vm.SelectMediaItem(index);
            vm.MediaControlsViewModel.PlayMedia();
        }
    }
}
