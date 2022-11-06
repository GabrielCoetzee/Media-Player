using System.ComponentModel;

namespace MediaPlayer.Settings
{
    public interface ISettingsManager : INotifyPropertyChanged
    {
        string BackgroundColor { get; }
        string BaseColor { get; set; }
        string Accent { get; set;  }
        decimal Opacity { get; set; }
        bool UpdateMetadata { get; set; }
        bool SaveMetadataToFile { get; set; }
        void SaveSettings();
    }
}
