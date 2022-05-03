using MediaPlayer.ViewModel.Commands.Abstract.EventTriggers;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete.EventTriggers
{
    public class SeekbarThumbStartedDraggingCommand : ISeekbarThumbStartedDraggingCommand
    {
        readonly ModelMediaPlayer _model;

        public SeekbarThumbStartedDraggingCommand(ModelMediaPlayer model)
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
            _model.IsUserDraggingSeekbarThumb = true;
        }
    }
}
