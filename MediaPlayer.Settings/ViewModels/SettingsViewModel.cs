using Generic.PropertyNotify;
using MediaPlayer.Settings.Configuration;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.ViewModels
{
    [Export]
    public class SettingsViewModel : NotifyPropertyChanged
    {
        readonly MetadataSettings _metadataSettings;

        [ImportingConstructor]
        public SettingsViewModel(MetadataSettings metadataSettings)
        {
            _metadataSettings = metadataSettings;
        }

        [Import]
        public ThemeViewModel ThemeViewModel { get; set; }

        public bool UpdateMetadata
        {
            get => _metadataSettings.UpdateMetadata;
            set
            {
                _metadataSettings.UpdateMetadata = value;
                OnPropertyChanged(nameof(UpdateMetadata));
            }
        }

        public bool SaveMetadataToFile
        {
            get => _metadataSettings.SaveMetadataToFile;
            set
            {
                _metadataSettings.SaveMetadataToFile = value;
                OnPropertyChanged(nameof(SaveMetadataToFile));
            }
        }

        public void SaveSettings()
        {
            ThemeViewModel.SaveSettings();
            _metadataSettings.Save();
        }
    }
}
