using Generic.NamedPipes.Wrappers;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.ViewModel.Services.Abstract;
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
        readonly IMetadataWriterService _metadataWriterService;

        [ImportingConstructor]
        public MainWindowClosingCommand(IMetadataWriterService metadataWriterService)
        {
            _metadataWriterService = metadataWriterService;
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

            vm.UpdateMetadataTokenSources.ForEach(x => x.Cancel());
            vm.UpdateMetadataTokenSources.Clear();

            if (vm.StopCommand.CanExecute(vm))
                vm.StopCommand.Execute(vm);

            vm.SelectedMediaItem = null;

            _metadataWriterService.WriteChangesToFilesInParallel(vm.MediaItems.Where(x => x.IsDirty));

            var pipeManager = new NamedPipeManager("MediaPlayer");
            await pipeManager.StopServerAsync();
        }
    }
}
