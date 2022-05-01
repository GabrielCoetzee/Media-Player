using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class NextTrackCommand : INextTrackCommand
    {
        readonly ModelMediaPlayer _model;

        public NextTrackCommand(ModelMediaPlayer model)
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
            return !this._model.IsMediaListEmpty() && (this._model.IsNextMediaItemAvailable() || _model.IsRepeatEnabled);
        }

        public void Execute(object parameter)
        {
            this._model.PlayNextMediaItem();
        }
    }
}
