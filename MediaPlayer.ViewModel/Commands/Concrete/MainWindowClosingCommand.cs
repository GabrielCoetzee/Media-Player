using Generic.NamedPipes.Wrappers;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.ViewModel.Commands.Abstract;
using MediaPlayer.ViewModel.Services.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.MainWindowClosing, typeof(ICommand))]
    public class MainWindowClosingCommand : ICommand
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

        public async void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            vm.UpdateMetadataTokenSources.ForEach(x => x.Cancel());
            vm.UpdateMetadataTokenSources.Clear();

            vm.StopMedia();

            vm.CurrentPositionTracker.Stop();
            vm.SelectedMediaItem = null;

            //await vm.SaveChangesAsync();

            var pipeManager = new NamedPipeManager("MediaPlayer");
            await pipeManager.StopServerAsync();
        }
    }
}
