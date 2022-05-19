using MediaPlayer.Common.Constants;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.Repeat, typeof(ICommand))]
    public class RepeatMediaListCommand : ICommand
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

            return vm.IsMediaListPopulated && vm.SelectedMediaItem != null;
        }

        public void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

            vm.IsRepeatEnabled = !vm.IsRepeatEnabled;
        }
    }
}
