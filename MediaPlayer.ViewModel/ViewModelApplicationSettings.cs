using Generic.PropertyNotify;
using MediaPlayer.ApplicationSettings;

namespace MediaPlayer.ViewModel
{
    public class ViewModelApplicationSettings : PropertyNotifyBase
    {
        #region Properties

        public ISettingsProvider SettingsProvider { get; set; }

        #endregion

        #region Constructor

        public ViewModelApplicationSettings(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider;
        }

        #endregion
    }
}
