using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.Mute, typeof(ICommand))]
    public class MuteCommand : ICommand
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

            return vm.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            vm.MediaVolume = vm.MediaVolume == VolumeLevel.Full ? VolumeLevel.Mute : VolumeLevel.Full;
        }
    }
}
