using ControlzEx.Theming;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MediaPlayer.ViewModel.EventTriggers.Abstract;
using MediaPlayer.Theming.Abstract;
using MediaPlayer.Settings;
using System.ComponentModel.Composition;
using Generic;
using MediaPlayer.Common.Constants;
using MediaPlayer.Settings.Config;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    [Export(CommandNames.LoadThemeOnWindowLoaded, typeof(ICommand))]
    public class LoadThemeOnWindowLoadedCommand : ICommand
    {
        readonly Configuration _configuration;
        readonly IThemeSelector _themeSelector;

        [ImportingConstructor]
        public LoadThemeOnWindowLoadedCommand(Configuration configuration, IThemeSelector themeSelector)
        {
            _configuration = configuration;
            _themeSelector = themeSelector;
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

            _themeSelector.ChangeAccent(_configuration.Accent);
        }
    }
}
