using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class ShuffleCommand : IShuffleCommand
    {
        readonly ModelMediaPlayer _model;

        public ShuffleCommand(ModelMediaPlayer model)
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
            return _model.MediaItems.Count > 2;
        }

        public void Execute(object parameter)
        {
            if (!_model.IsMediaItemsShuffled)
            {
                this._model.ShuffleMediaList();

                return;
            }

            this._model.OrderMediaList();
        }
    }
}
