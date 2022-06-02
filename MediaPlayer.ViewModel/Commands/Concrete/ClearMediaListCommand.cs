using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition;
using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Linq;
using MediaPlayer.ViewModel.Commands.Abstract;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.ClearList, typeof(ICommand))]
    public class ClearMediaListCommand : ICommand
    {
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

        public async void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            vm.UpdateMetadataTokenSources.ForEach(x => x.Cancel());
            vm.UpdateMetadataTokenSources.Clear();

            vm.StopMedia();

            vm.CurrentPositionTracker.Stop();
            vm.SelectedMediaItem = null;

            await vm.SaveChangesAsync();

            vm.MediaItems.Clear();
            vm.MediaItems = new Model.Collections.MediaItemObservableCollection();

            vm.BusyViewModel.MediaListTitle = string.Empty;
            vm.BusyViewModel.IsLoading = false;
        }
    }
}
