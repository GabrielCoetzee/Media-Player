using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;
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

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return;

            if (vm.IsMuted)
            {
                vm.MediaVolume = vm.PreMuteVolume > 0 ? vm.PreMuteVolume : 1.0;
                return;
            }

            vm.PreMuteVolume = vm.MediaVolume;
            vm.MediaVolume = 0.0;
        }
    }
}
