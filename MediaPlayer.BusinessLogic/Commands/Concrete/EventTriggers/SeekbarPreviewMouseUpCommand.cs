using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.BusinessLogic.State.Abstract;
using MediaPlayer.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
{
    public class SeekbarPreviewMouseUpCommand : ISeekbarPreviewMouseUpCommand
    {
        readonly IState _state;

        public SeekbarPreviewMouseUpCommand(IState state)
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
            return _state.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MouseButtonEventArgs e)
                return;

            var seekbar = e.Source as Slider;

            var pointerLocation = (e.GetPosition(seekbar).X / seekbar.ActualWidth) * (seekbar.Maximum - seekbar.Minimum);

            _state.MediaElementPosition = TimeSpan.FromSeconds(pointerLocation);
        }
    }
}
