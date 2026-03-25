using MediaPlayer.Common.Constants;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.PlayPause, typeof(ICommand))]
    public class PlayPauseCommand : ICommand
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

            if (vm.MediaControlsViewModel.MediaState == MediaState.Play)
            {
                vm.MediaControlsViewModel.SetPlaybackState(MediaState.Pause);
                return;
            }

            vm.MediaControlsViewModel.SetPlaybackState(MediaState.Play);
        }
    }
}
