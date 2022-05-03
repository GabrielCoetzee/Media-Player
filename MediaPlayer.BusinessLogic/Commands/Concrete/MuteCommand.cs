using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
using MediaPlayer.Common.Enumerations;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class MuteCommand : IMuteCommand
    {
        readonly IState _state;

        public MuteCommand(IState state)
        {
            _state = state;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _state.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            _state.MediaVolume = _state.MediaVolume == VolumeLevel.Full ? VolumeLevel.Mute : VolumeLevel.Full;
        }
    }
}
