using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition;
using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Linq;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.ClearList, typeof(ICommand))]
    public class ClearMediaListCommand : ICommand
    {
        readonly IMetadataWriterService _metadataWriterService;

        [ImportingConstructor]
        public ClearMediaListCommand(IMetadataWriterService metadataWriterService)
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
            if (parameter is not MainViewModel vm)
                return false;

            return vm.IsMediaListPopulated;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            if (vm.StopCommand.CanExecute(vm))
                vm.StopCommand.Execute(vm);

            vm.UpdateMetadataTokenSources.ForEach(x => x.Cancel());
            vm.UpdateMetadataTokenSources.Clear();

            vm.CurrentPositionTracker.Stop();
            vm.SelectedMediaItem = null;

            _metadataWriterService.WriteChangesToFilesInParallel(vm.MediaItems.Where(x => x.IsDirty));

            vm.MediaState = MediaState.Stop;
            vm.MediaItems.Clear();
            vm.MediaItems = new Model.Collections.MediaItemObservableCollection();

            vm.BusyViewModel.MediaListTitle = string.Empty;
            vm.BusyViewModel.IsLoading = false;
        }
    }
}
