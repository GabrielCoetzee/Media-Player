using Generic.PropertyNotify;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.ViewModel.Commands.Abstract.EventTriggers;

namespace MediaPlayer.ViewModel
{
    public class ViewModelApplicationSettings : PropertyNotifyBase
    {
        #region Properties

        public ISettingsProvider SettingsProvider { get; set; }
        public ILoadThemeOnWindowLoadedCommand LoadThemeOnWindowLoadedCommand { get; set; }
        public ISaveSettingsCommand SaveSettingsCommand { get; set; }

        #endregion

        #region Constructor

        public ViewModelApplicationSettings(ISettingsProvider settingsProvider,
            ILoadThemeOnWindowLoadedCommand loadThemeOnWindowLoadedCommand,
            ISaveSettingsCommand saveSettingsCommand)
        {
            SettingsProvider = settingsProvider;
            LoadThemeOnWindowLoadedCommand = loadThemeOnWindowLoadedCommand;
            SaveSettingsCommand = saveSettingsCommand;
        }

        #endregion
    }
}
