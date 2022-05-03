using MediaPlayer.ApplicationSettings;
using MediaPlayer.ViewModel.Commands.Abstract.EventTriggers;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using MediaPlayer.Model.Implementation;

namespace MediaPlayer.ViewModel.Commands.Concrete.EventTriggers
{
    public class TopMostGridDropCommand : ITopMostGridDropCommand
    {
        readonly MetadataReaderResolver _metadataReaderResolver;
        readonly ISettingsProvider _settingsProvider;

        public TopMostGridDropCommand(MetadataReaderResolver metadataReaderResolver, 
            ISettingsProvider settingsProvider)
        {
            _metadataReaderResolver = metadataReaderResolver;
            _settingsProvider = settingsProvider;
        }

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
