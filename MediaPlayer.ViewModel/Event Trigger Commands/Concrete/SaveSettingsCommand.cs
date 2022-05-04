using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.ViewModel.EventTriggers.Abstract;
using System;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    public class SaveSettingsCommand : ISaveSettingsCommand
    {
        readonly ISettingsProvider _settingsProvider;

        public SaveSettingsCommand(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
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

        public void Execute(object parameter)
        {
            if (parameter is not MetroWindow applicationSettingsWindow)
                return;

            _settingsProvider.SaveSettings();

            applicationSettingsWindow.Close();
        }
    }
}
