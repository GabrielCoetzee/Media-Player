using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Generic.Mediator;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    [Export(CommandNames.TopMostGridDrop, typeof(ICommand))]
    public class TopMostGridDropCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is not DragEventArgs e)
                return;

            var droppedContent = (IEnumerable)e.Data.GetData(DataFormats.FileDrop);

            if (droppedContent == null)
                return;

            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.ProcessContent, droppedContent);
        }
    }
}
