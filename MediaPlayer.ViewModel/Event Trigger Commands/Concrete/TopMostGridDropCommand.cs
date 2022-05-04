using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using MediaPlayer.ViewModel.EventTriggers.Abstract;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    public class TopMostGridDropCommand : ITopMostGridDropCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public event EventHandler<ProcessDroppedContentEventArgs> ProcessDroppedContent;

        public virtual void OnProcessDroppedContent(ProcessDroppedContentEventArgs e)
        {
            ProcessDroppedContent?.Invoke(this, e);
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

            OnProcessDroppedContent(new ProcessDroppedContentEventArgs() { FilePaths = droppedContent });
        }
    }

    public class ProcessDroppedContentEventArgs : EventArgs
    {
        public IEnumerable FilePaths { get; set; }
    }
}
