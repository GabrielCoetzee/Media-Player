using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;
using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(nameof(CommandNames.StartedDragging), typeof(ICommand))]
    public class SeekbarThumbStartedDraggingCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return false;

            return vm.MediaState == MediaState.Play;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MediaControlsViewModel vm)
                return;

            vm.IsUserDraggingSeekbarThumb = true;
        }
    }
}
