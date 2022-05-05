using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.ViewModel.EventTriggers.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    public class SaveSettingsCommand : ISaveSettingsCommand
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

            vm.SettingsProviderViewModel.SaveSettings();

            applicationSettingsWindow.Close();
        }
    }
}
