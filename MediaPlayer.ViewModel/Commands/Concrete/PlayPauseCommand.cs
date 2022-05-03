using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class PlayPauseCommand : IPlayPauseCommand
    {
        readonly ModelMediaPlayer _model;

        public PlayPauseCommand(ModelMediaPlayer model)
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
            if (_model.MediaState != MediaState.Play)
            {
                _model.PlayMedia();
                return;
            }

            _model.PauseMedia();
        }
    }
}
