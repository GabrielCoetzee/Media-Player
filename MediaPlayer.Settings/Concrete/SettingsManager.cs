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
    public class SettingsManager : NotifyPropertyChanged, ISettingsManager
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

            _baseColor = _applicationSettings.BaseColor;
            _accent = _applicationSettings.Accent;
            _opacity = _applicationSettings.Opacity;

            _updateMetadata = _metadataSettings.UpdateMetadata;
            _saveMetadataToFile = _metadataSettings.SaveMetadataToFile;
        }

        private string _baseColor;
        private string _accent;
        private decimal _opacity;
        private bool _updateMetadata;
        private bool _saveMetadataToFile;

        public string BackgroundColor => BaseColor == "Dark" ? "Black" : "White";
        public string BackgroundColorInverse => BaseColor == "Dark" ? "White" : "Black";

        public string BaseColor
        {
            get => _baseColor;
            set
            {
                _baseColor = value;
                OnPropertyChanged(nameof(BaseColor));
                OnPropertyChanged(nameof(BackgroundColor));

                _applicationSettings.BaseColor = value;
                _themeManager.ChangeTheme(value, Accent);
            }
        }

        public string Accent
        {
            get => _accent;
            set
            {
                _accent = value;
                OnPropertyChanged(nameof(Accent));

                _applicationSettings.Accent = value;
                _themeManager.ChangeTheme(BaseColor, value);
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

        public bool UpdateMetadata 
        {
            get => _updateMetadata;
            set
            {
                _updateMetadata = value;
                OnPropertyChanged(nameof(UpdateMetadata));

                _metadataSettings.UpdateMetadata = value;
            }
        }

        public bool SaveMetadataToFile
        {
            get => _saveMetadataToFile;
            set
            {
                _saveMetadataToFile = value;
                OnPropertyChanged(nameof(SaveMetadataToFile));

                _metadataSettings.SaveMetadataToFile = value;
            }
        }

        public void SaveSettings()
        {
            _applicationSettings.Save();
            _metadataSettings.Save();
        }

    }
}
