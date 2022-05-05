using Generic.PropertyNotify;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.ViewModel.EventTriggers.Abstract;

namespace MediaPlayer.ViewModel
{
    public class ApplicationSettingsViewModel : PropertyNotifyBase
    {
        public ISettingsProviderViewModel SettingsProviderViewModel { get; set; }
        public ILoadThemeOnWindowLoadedCommand LoadThemeOnWindowLoadedCommand { get; set; }
        public ISaveSettingsCommand SaveSettingsCommand { get; set; }

        public ApplicationSettingsViewModel(ISettingsProviderViewModel settingsProviderViewModel,
            ILoadThemeOnWindowLoadedCommand loadThemeOnWindowLoadedCommand,
            ISaveSettingsCommand saveSettingsCommand)
        {
            SettingsProviderViewModel = settingsProviderViewModel;
            LoadThemeOnWindowLoadedCommand = loadThemeOnWindowLoadedCommand;
            SaveSettingsCommand = saveSettingsCommand;
        }
    }
}
