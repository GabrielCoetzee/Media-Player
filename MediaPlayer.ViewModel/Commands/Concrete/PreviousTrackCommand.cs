using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class PreviousTrackCommand : IPreviousTrackCommand
    {
        readonly ModelMediaPlayer _model;

        public PreviousTrackCommand(ModelMediaPlayer model)
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
            return !_model.IsMediaListEmpty() && (_model.IsPreviousMediaItemAvailable() || _model.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            _model.PlayPreviousMediaItem();
        }
    }
}
