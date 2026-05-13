using Generic.PropertyNotify;
using MediaPlayer.Settings.Configuration;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.ViewModels
{
    [Export]
    public class SettingsViewModel : NotifyPropertyChanged
    {
        public MetadataSettings MetadataSettings { get; }
        public ThemeViewModel ThemeViewModel { get; }

        [ImportingConstructor]
        public SettingsViewModel(MetadataSettings metadataSettings, ThemeViewModel themeViewModel)
        {
            MetadataSettings = metadataSettings;
            ThemeViewModel = themeViewModel;
        }

        public bool UpdateMetadata
        {
            get => MetadataSettings.UpdateMetadata;
            set
            {
                MetadataSettings.UpdateMetadata = value;
                OnPropertyChanged(nameof(UpdateMetadata));
                MetadataSettings.Save();
            }
        }

        public bool SaveMetadataToFile
        {
            get => MetadataSettings.SaveMetadataToFile;
            set
            {
                MetadataSettings.SaveMetadataToFile = value;
                OnPropertyChanged(nameof(SaveMetadataToFile));
                MetadataSettings.Save();
            }
        }
    }
}
