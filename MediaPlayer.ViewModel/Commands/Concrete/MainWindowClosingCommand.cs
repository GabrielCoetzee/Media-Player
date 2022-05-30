using Generic.NamedPipes.Wrappers;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Metadata.Concrete;
using System;
using System.Collections.Generic;
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

        public async void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            await WriteChangesToFilesAsync(vm.MediaItems.Where(x => x.IsDirty));

            var pipeManager = new NamedPipeManager("MediaPlayer");
            await pipeManager.StopServerAsync();
        }

        private async Task WriteChangesToFilesAsync(IEnumerable<MediaItem> mediaItems)
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(mediaItems, (x) => _metadataWriterFactory.Resolve(MetadataLibraries.Taglib).Update(x));
            });
        }
    }
}
