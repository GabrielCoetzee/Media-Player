using Generic.NamedPipes.Wrappers;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Concrete;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.MainWindowClosing, typeof(ICommand))]
    public class MainWindowClosingCommand : ICommand
    {
        readonly MetadataWriterFactory _metadataWriterFactory;

        [ImportingConstructor]
        public MainWindowClosingCommand(MetadataWriterFactory metadataWriterFactory)
        {
            _metadataWriterFactory = metadataWriterFactory;
        }

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
            if (parameter is not MainViewModel vm)
                return;

            Parallel.ForEach(vm.MediaItems.Where(x => x.IsDirty), (x) => x.Update(_metadataWriterFactory.Resolve(MetadataLibraries.Taglib)));

            var pipeManager = new NamedPipeManager("MediaPlayer");
            pipeManager.StopServer();
        }
    }
}
