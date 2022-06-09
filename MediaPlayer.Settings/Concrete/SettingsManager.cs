using Generic.PropertyNotify;
using MediaPlayer.Settings.Config;
using MediaPlayer.Settings.Configuration;
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
        readonly MetadataSettings _metadataSettings;
        readonly IThemeManager _themeManager;

        [ImportingConstructor]
        public SettingsManager(ApplicationSettings applicationSettings,
            MetadataSettings metadataSettings,
            IThemeManager themeManager)
        {
            _applicationSettings = applicationSettings;
            _metadataSettings = metadataSettings;
            _themeManager = themeManager;

            _opacity = _applicationSettings.Opacity;
            _accent = _applicationSettings.Accent;

            _isUpdateMetadataEnabled = _metadataSettings.IsUpdateMetadataEnabled;
            _isSaveMetadataToFileEnabled = _metadataSettings.IsSaveMetadataToFileEnabled;
        }

        private decimal _opacity;
        private string _accent;
        private bool _isUpdateMetadataEnabled;
        private bool _isSaveMetadataToFileEnabled;

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

        public bool IsUpdateMetadataEnabled 
        {
            get => _isUpdateMetadataEnabled;
            set
            {
                _isUpdateMetadataEnabled = value;
                OnPropertyChanged(nameof(IsUpdateMetadataEnabled));

                _metadataSettings.IsUpdateMetadataEnabled = value;
            }
        }

        public bool IsSaveMetadataToFileEnabled
        {
            get => _isSaveMetadataToFileEnabled;
            set
            {
                _isSaveMetadataToFileEnabled = value;
                OnPropertyChanged(nameof(IsSaveMetadataToFileEnabled));

                _metadataSettings.IsSaveMetadataToFileEnabled = value;
            }
        }

        public void SaveSettings()
        {
            _applicationSettings.Save();
            _metadataSettings.Save();
        }

    }
}
