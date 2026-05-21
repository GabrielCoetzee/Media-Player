using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ConverterObject;

namespace MediaPlayer.ViewModel.Commands
{
    [Export(CommandNames.RemoveMediaItem, typeof(ICommand))]
    public class RemoveMediaItemCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is RemoveMediaItemConverterModel model
                && model.QueueViewModel != null
                && model.MediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not RemoveMediaItemConverterModel model)
                return;

            model.QueueViewModel.RemoveMediaItem(model.MediaItem);
        }
    }
}
