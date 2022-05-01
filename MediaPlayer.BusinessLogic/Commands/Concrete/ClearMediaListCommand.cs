using MediaPlayer.BusinessEntities.Collections;
using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
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
            return !this._model.IsMediaListEmpty();
        }

        public void Execute(object parameter)
        {
            this._model._mediaPositionTracker.Stop();

            this._model.MediaVolume = VolumeLevel.Full;
            this._model.MediaState = MediaState.Stop;
            this._model.MediaItems = new MediaItemObservableCollection();
        }
    }
}
