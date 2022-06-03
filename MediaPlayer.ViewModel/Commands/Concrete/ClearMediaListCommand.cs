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
using Generic.Mediator;
using MediaPlayer.Common.Enumerations;

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

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            var shutdownApplication = false;

            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.SaveChangesToDirtyFiles, shutdownApplication);
        }
    }
}
