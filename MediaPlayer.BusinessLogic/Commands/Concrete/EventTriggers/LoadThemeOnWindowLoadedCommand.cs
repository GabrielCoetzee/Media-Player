using ControlzEx.Theming;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.Theming;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
{
    public class LoadThemeOnWindowLoadedCommand : ILoadThemeOnWindowLoadedCommand
    {
        readonly ISettingsProvider _settingsProvider;
        readonly IThemeSelector _themeSelector;

        public LoadThemeOnWindowLoadedCommand(ISettingsProvider settingsProvider,
            IThemeSelector themeSelector)
        {
            _settingsProvider = settingsProvider;
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

            _themeSelector.ChangeAccent(_settingsProvider.SelectedAccent);
        }
    }
}
