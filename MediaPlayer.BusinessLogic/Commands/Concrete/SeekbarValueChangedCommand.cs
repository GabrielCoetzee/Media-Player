using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class SeekbarValueChangedCommand : ISeekbarValueChangedCommand
    {
        readonly ModelMediaPlayer _model;

        public SeekbarValueChangedCommand(ModelMediaPlayer model)
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
            if (_model.IsUserDraggingSeekbarThumb)
                this._model.CurrentPosition = this._model.SelectedMediaItem.ElapsedTime;
        }
    }
}
