﻿using ControlzEx.Theming;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition;
using Generic;
using MediaPlayer.Common.Constants;
using MediaPlayer.Settings.Config;
using MediaPlayer.Settings.ViewModels;
using MediaPlayer.ViewModel.ConverterObject;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.LoadAccentOptionsCommand, typeof(ICommand))]
    public class LoadAccentOptionsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not LoadThemeConverterModel model)
                return false;

            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is not LoadThemeConverterModel model)
                return;

            ThemeManager.Current.ColorSchemes.ToList().ForEach(accent => model.ComboBox.Items.Add(accent));

            model.ThemeViewModel.ChangeTheme();
        }
    }
}