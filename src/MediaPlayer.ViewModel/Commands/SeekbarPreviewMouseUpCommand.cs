using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ConverterObject;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands
{
    [Export(CommandNames.SeekbarPreviewMouseUp, typeof(ICommand))]
    public class SeekbarPreviewMouseUpCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is SeekConverterModel model
                && model.MediaControlsViewModel != null
                && model.Seekbar != null
                && model.Seekbar.ActualWidth > 0;
        }

        public void Execute(object parameter)
        {
            if (parameter is not SeekConverterModel model)
                return;

            var seekbar = model.Seekbar;

            var cursorX = Mouse.GetPosition(seekbar).X;
            var seconds = (cursorX / seekbar.ActualWidth) * (seekbar.Maximum - seekbar.Minimum);

            model.MediaControlsViewModel.Seek(TimeSpan.FromSeconds(seconds));
        }
    }
}
