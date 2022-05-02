using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
