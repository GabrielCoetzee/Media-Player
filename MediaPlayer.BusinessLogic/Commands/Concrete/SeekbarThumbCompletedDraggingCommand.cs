using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class SeekbarThumbCompletedDraggingCommand : ISeekbarThumbCompletedDraggingCommand
    {
        readonly ModelMediaPlayer _model;

        public SeekbarThumbCompletedDraggingCommand(ModelMediaPlayer model)
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
            return _model.IsUserDraggingSeekbarThumb;
        }

        public void Execute(object parameter)
        {
            _model.IsUserDraggingSeekbarThumb = false;
        }
    }
}
