using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class MuteCommand : IMuteCommand
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

            return vm.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return;

            vm.MediaVolume = vm.MediaVolume == VolumeLevel.Full ? VolumeLevel.Mute : VolumeLevel.Full;
        }
    }
}
