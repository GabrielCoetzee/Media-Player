using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using MediaPlayer.Model.Collections;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class ClearMediaListCommand : IClearMediaListCommand
    {
        readonly IState _state;

        public ClearMediaListCommand(IState state)
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
            return !_state.IsMediaListEmpty();
        }

        public void Execute(object parameter)
        {
            _state.CurrentPositionTracker.Stop();

            _state.MediaVolume = VolumeLevel.Full;
            _state.MediaState = MediaState.Stop;
            _state.MediaItems = new MediaItemObservableCollection();
        }
    }
}
