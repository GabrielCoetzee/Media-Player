using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class StopCommand : IStopCommand
    {
        readonly ModelMediaPlayer _model;

        public StopCommand(ModelMediaPlayer model)
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
            return !_model.IsMediaListEmpty() && _model.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            _model.SelectMediaItem(_model.GetFirstMediaItemIndex());

            _model.StopMedia();
        }
    }
}
