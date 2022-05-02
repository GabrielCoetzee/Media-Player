using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
{
    public class SeekbarPreviewMouseUpCommand : ISeekbarPreviewMouseUpCommand
    {
        readonly ModelMediaPlayer _model;

        public SeekbarPreviewMouseUpCommand(ModelMediaPlayer model)
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
            if (parameter is not MouseButtonEventArgs e)
                return;

            var seekbar = e.Source as Slider;

            var pointerLocation = (e.GetPosition(seekbar).X / seekbar.ActualWidth) * (seekbar.Maximum - seekbar.Minimum);

            _model.MediaElementPosition = TimeSpan.FromSeconds(pointerLocation);
        }
    }
}
