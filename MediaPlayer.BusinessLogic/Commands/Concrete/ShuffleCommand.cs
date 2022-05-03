using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.State.Abstract;
using MediaPlayer.Model;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class ShuffleCommand : IShuffleCommand
    {
        readonly IState _state;

        public ShuffleCommand(IState state)
        {
            _state = state;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _state.MediaItems.Count > 2;
        }

        public void Execute(object parameter)
        {
            if (!_state.IsMediaItemsShuffled)
            {
                _state.ShuffleMediaList();

                return;
            }

            _state.OrderMediaList();
        }
    }
}
