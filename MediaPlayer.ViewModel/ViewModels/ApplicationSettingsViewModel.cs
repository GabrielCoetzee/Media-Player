using Generic.DependencyInjection;
using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.Settings.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel
{
    [Export]
    public class ApplicationSettingsViewModel : NotifyPropertyChanged
    {
        [Import]
        public SettingsViewModel SettingsViewModel { get; set; }

        [Import(CommandNames.LoadAccentOptionsCommand)]
        public ICommand LoadAccentOptionsCommand { get; set; }

        [Import(CommandNames.LoadThemeOptionsCommand)]
        public ICommand LoadThemeOptionsCommand { get; set; }

        [Import(CommandNames.SaveSettings)]
        public ICommand SaveSettingsCommand { get; set; }
    }
}
