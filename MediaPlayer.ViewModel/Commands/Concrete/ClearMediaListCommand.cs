using MediaPlayer.Model.Collections;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using MediaPlayer.ViewModel.Commands.Abstract;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class ClearMediaListCommand : IClearMediaListCommand
    {
        public ClearMediaListCommand()
        {
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return false;

            return !vm.IsMediaListEmpty();
        }

        public void Execute(object parameter)
        {
            if (parameter is not ViewModelMediaPlayer vm)
                return;

            vm.CurrentPositionTracker.Stop();

            vm.MediaVolume = VolumeLevel.Full;
            vm.MediaState = MediaState.Stop;
            vm.MediaItems.Clear();
        }
    }
}
