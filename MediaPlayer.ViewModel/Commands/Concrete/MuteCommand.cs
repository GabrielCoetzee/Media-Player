using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class MuteCommand : IMuteCommand
    {
        readonly ModelMediaPlayer _model;

        public MuteCommand(ModelMediaPlayer model)
        {
            _model = model;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _model.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            _model.MediaVolume = _model.MediaVolume == VolumeLevel.Full ? VolumeLevel.Mute : VolumeLevel.Full;
        }
    }
}
