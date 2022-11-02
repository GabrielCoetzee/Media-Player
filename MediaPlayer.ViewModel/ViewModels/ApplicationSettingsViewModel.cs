using Generic.DependencyInjection;
using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.Settings;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.ViewModel
{
    [Export]
    public class ApplicationSettingsViewModel : NotifyPropertyChanged
    {
        [Import]
        public ISettingsManager SettingsManager { get; set; }

        [Import(CommandNames.LoadThemeOnWindowLoaded)]
        public ICommand LoadThemeOnWindowLoadedCommand { get; set; }

        [Import(CommandNames.SaveSettings)]
        public ICommand SaveSettingsCommand { get; set; }
    }
}
