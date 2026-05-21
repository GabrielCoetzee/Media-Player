using MediaPlayer.Common.Constants;
using MediaPlayer.ViewModel.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands
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
            if (parameter is not QueueViewModel vm)
                return false;

            return vm.IsMediaListPopulated;
        }

        public async void Execute(object parameter)
        {
            if (parameter is not QueueViewModel vm)
                return;

            await vm.ClearMediaListAsync();
        }
    }
}
