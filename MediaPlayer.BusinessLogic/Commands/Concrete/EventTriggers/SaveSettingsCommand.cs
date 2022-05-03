using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using System;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
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
