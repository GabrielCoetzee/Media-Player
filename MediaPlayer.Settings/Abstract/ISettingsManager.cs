using System.ComponentModel;

namespace MediaPlayer.Settings.Abstract
{
    public interface ISettingsManager : INotifyPropertyChanged
    {
        bool UpdateMetadata { get; set; }
        bool SaveMetadataToFile { get; set; }
        void SaveSettings();
    }
}
