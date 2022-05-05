using ControlzEx.Theming;
using MediaPlayer.ApplicationSettings;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MediaPlayer.ViewModel.EventTriggers.Abstract;
using MediaPlayer.Theming.Abstract;

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    public class LoadThemeOnWindowLoadedCommand : ILoadThemeOnWindowLoadedCommand
    {
        readonly ISettingsProviderViewModel _settingsProvider;
        readonly IThemeSelector _themeSelector;

        public LoadThemeOnWindowLoadedCommand(ISettingsProviderViewModel settingsProvider,
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
