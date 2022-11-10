using ControlzEx.Theming;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MediaPlayer.Settings;
using System.ComponentModel.Composition;
using Generic;
using MediaPlayer.Common.Constants;
using MediaPlayer.Settings.Config;
using MediaPlayer.Settings.Abstract;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.LoadThemeOptionsCommand, typeof(ICommand))]
    public class LoadThemeOptionsCommand : ICommand
    {
        readonly ThemeSettings _themeSettings;
        readonly IThemeManager _themeManager;

        [ImportingConstructor]
        public LoadThemeOptionsCommand(ThemeSettings themeSettings, IThemeManager themeManager)
        {
            _themeSettings = themeSettings;
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
            if (parameter is ComboBox comboBoxThemes)
                ThemeManager.Current.BaseColors.ToList().ForEach(accent => comboBoxThemes.Items.Add(accent));

            _themeManager.ChangeTheme(_themeSettings.BaseColor, _themeSettings.Accent);
        }
    }
}
