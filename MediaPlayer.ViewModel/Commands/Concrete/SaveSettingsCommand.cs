using MahApps.Metro.Controls;
using MediaPlayer.Common.Constants;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.SaveSettings, typeof(ICommand))]
    public class SaveSettingsCommand : ICommand
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

        public void Execute(object parameter)
        {
            if (parameter is not MetroWindow applicationSettingsWindow)
                return;

            if (applicationSettingsWindow.DataContext is not ApplicationSettingsViewModel vm)
                return;

            vm.ThemeViewModel.SaveSettings();
            vm.SettingsViewModel.SaveSettings();

            applicationSettingsWindow.Close();
        }
    }
}
