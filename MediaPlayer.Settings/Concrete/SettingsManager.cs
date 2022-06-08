using Generic.PropertyNotify;
using MediaPlayer.Settings.Config;
using MediaPlayer.Theming.Abstract;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.Concrete
{
    /// <summary>
    /// Intermediary Manager for settings that notifies property changes.
    /// Don't want to also make serializable settings implement property notify 
    /// base
    /// </summary>
    [Export(typeof(ISettingsManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsManager : PropertyNotifyBase, ISettingsManager
    {
        readonly ApplicationSettings _applicationSettings;
        readonly IThemeManager _themeManager;

        [ImportingConstructor]
        public SettingsManager(ApplicationSettings applicationSettings,
            IThemeManager themeManager)
        {
            _applicationSettings = applicationSettings;
            _themeManager = themeManager;

            _opacity = _applicationSettings.Opacity;
            _accent = _applicationSettings.Accent;
        }

        private decimal _opacity;
        private string _accent;

        public string Accent
        {
            get => _accent;
            set
            {
                _accent = value;
                OnPropertyChanged(nameof(Accent));

                _applicationSettings.Accent = value;
                _themeManager.ChangeAccent(value);
            }
        }

        public decimal Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));

                _applicationSettings.Opacity = value;
                _themeManager.ChangeOpacity((double)value);
            }
        }

        public void SaveSettings()
        {
            _applicationSettings.Save();
        }

    }
}
