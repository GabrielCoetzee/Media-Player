using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
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
                this._model.PlayMedia();
                return;
            }

            this._model.PauseMedia();
        }
    }
}
