using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands
{
    [Export(CommandNames.Mute, typeof(ICommand))]
    public class MuteCommand : ICommand
    {
        private double _preMuteVolume = 1.0;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return;

            if (vm.IsMuted)
            {
                vm.MediaVolume = _preMuteVolume > 0 ? _preMuteVolume : 1.0;
                return;
            }

            _preMuteVolume = vm.MediaVolume;
            vm.MediaVolume = 0.0;
        }
    }
}
