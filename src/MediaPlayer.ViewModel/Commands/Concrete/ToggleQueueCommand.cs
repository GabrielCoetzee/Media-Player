using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.ToggleQueue, typeof(ICommand))]
    public class ToggleQueueCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            vm.IsQueueOpen = !vm.IsQueueOpen;
        }
    }
}
