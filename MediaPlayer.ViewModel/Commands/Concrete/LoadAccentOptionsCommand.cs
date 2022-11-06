﻿using ControlzEx.Theming;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MediaPlayer.Theming.Abstract;
using MediaPlayer.Settings;
using System.ComponentModel.Composition;
using Generic;
using MediaPlayer.Common.Constants;
using MediaPlayer.Settings.Config;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.LoadAccentOptionsCommand, typeof(ICommand))]
    public class LoadAccentOptionsCommand : ICommand
    {
        readonly ApplicationSettings _applicationSettings;
        readonly IThemeManager _themeManager;

        [ImportingConstructor]
        public LoadAccentOptionsCommand(ApplicationSettings applicationSettings, IThemeManager themeManager)
        {
            _applicationSettings = applicationSettings;
            _themeManager = themeManager;
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
            if (parameter is ComboBox comboBoxAccents)
                ThemeManager.Current.ColorSchemes.ToList().ForEach(accent => comboBoxAccents.Items.Add(accent));

            _themeManager.ChangeTheme(_applicationSettings.BaseColor, _applicationSettings.Accent);
        }
    }
}
