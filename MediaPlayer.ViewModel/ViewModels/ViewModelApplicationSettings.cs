using Generic.PropertyNotify;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.ViewModel.EventTriggers.Abstract;

namespace MediaPlayer.ViewModel
{
    public class ViewModelApplicationSettings : PropertyNotifyBase
    {
        public ISettingsProvider SettingsProvider { get; set; }
        public ILoadThemeOnWindowLoadedCommand LoadThemeOnWindowLoadedCommand { get; set; }
        public ISaveSettingsCommand SaveSettingsCommand { get; set; }

        public ViewModelApplicationSettings(ISettingsProvider settingsProvider,
            ILoadThemeOnWindowLoadedCommand loadThemeOnWindowLoadedCommand,
            ISaveSettingsCommand saveSettingsCommand)
        {
            SettingsProvider = settingsProvider;
            LoadThemeOnWindowLoadedCommand = loadThemeOnWindowLoadedCommand;
            SaveSettingsCommand = saveSettingsCommand;
        }
    }
}
