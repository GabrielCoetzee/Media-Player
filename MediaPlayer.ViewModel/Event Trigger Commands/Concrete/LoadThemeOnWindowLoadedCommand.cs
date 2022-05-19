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

namespace MediaPlayer.ViewModel.EventTriggers.Concrete
{
    [Export(CommandNames.LoadThemeOnWindowLoaded, typeof(ICommand))]
    public class LoadThemeOnWindowLoadedCommand : ICommand
    {
        [Import]
        public ISettingsManager SettingsManager { get; set; }

        [Import]
        public IThemeSelector ThemeSelector { get; set; }

        public LoadThemeOnWindowLoadedCommand()
        {
            MEF.Container?.SatisfyImportsOnce(this);
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

            ThemeSelector.ChangeAccent(SettingsManager.SelectedAccent);
        }
    }
}
