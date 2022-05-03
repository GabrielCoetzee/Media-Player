using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
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
        readonly IState _state;

        public PlayPauseCommand(IState state)
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
            if (_state.MediaState != MediaState.Play)
            {
                _state.PlayMedia();
                return;
            }

            _state.PauseMedia();
        }
    }
}
