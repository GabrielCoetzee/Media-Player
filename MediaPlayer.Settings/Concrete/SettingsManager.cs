using Generic.PropertyNotify;
using MediaPlayer.Settings.Abstract;
using MediaPlayer.Settings.Config;
using MediaPlayer.Settings.Configuration;
using System.ComponentModel.Composition;
using System.Drawing;

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

        [ImportingConstructor]
        public SettingsManager(ApplicationSettings applicationSettings,
            MetadataSettings metadataSettings)
        {
            _applicationSettings = applicationSettings;
            _metadataSettings = metadataSettings;

            _updateMetadata = _metadataSettings.UpdateMetadata;
            _saveMetadataToFile = _metadataSettings.SaveMetadataToFile;
        }

        private bool _updateMetadata;
        private bool _saveMetadataToFile;

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
