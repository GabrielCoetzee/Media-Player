using MediaPlayer.Model;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
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
                _model.ShuffleMediaList();

                return;
            }

            _model.OrderMediaList();
        }
    }
}
