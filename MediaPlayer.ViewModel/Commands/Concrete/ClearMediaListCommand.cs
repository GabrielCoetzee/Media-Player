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
        readonly ModelMediaPlayer _model;

        public ClearMediaListCommand(ModelMediaPlayer model)
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
            return !_model.IsMediaListEmpty();
        }

        public void Execute(object parameter)
        {
            _model.CurrentPositionTracker.Stop();

            _model.MediaVolume = VolumeLevel.Full;
            _model.MediaState = MediaState.Stop;
            _model.MediaItems = new MediaItemObservableCollection();
        }
    }
}
