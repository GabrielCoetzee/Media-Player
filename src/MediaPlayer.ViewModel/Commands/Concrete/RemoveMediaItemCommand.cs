using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Generic.Mediator;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.RemoveMediaItem, typeof(ICommand))]
    public class RemoveMediaItemCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => parameter is MediaItem;

        public void Execute(object parameter)
        {
            if (parameter is not MediaItem item)
                return;

            Messenger<MessengerMessages>.Send(MessengerMessages.RemoveMediaItem, item);
        }
    }
}
