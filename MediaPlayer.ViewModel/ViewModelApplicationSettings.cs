using Generic.PropertyNotify;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;

namespace MediaPlayer.ViewModel
{
    public class ViewModelApplicationSettings : PropertyNotifyBase
    {
        #region Properties

        public ISettingsProvider SettingsProvider { get; set; }
        public ILoadThemeOnWindowLoadedCommand LoadThemeOnWindowLoadedCommand { get; set; }

        #endregion

        #region Constructor

        public ViewModelApplicationSettings(ISettingsProvider settingsProvider,
            ILoadThemeOnWindowLoadedCommand loadThemeOnWindowLoadedCommand)
        {
            SettingsProvider = settingsProvider;
            LoadThemeOnWindowLoadedCommand = loadThemeOnWindowLoadedCommand;
        }

        #endregion
    }
}
