using System.ComponentModel;

namespace MediaPlayer.Settings
{
    public interface ISettingsManager : INotifyPropertyChanged
    {
        string Accent { get; set;  }
        decimal Opacity { get; set; }
        bool IsUpdateMetadataEnabled { get; set; }
        bool IsSaveMetadataToFileEnabled { get; set; }
        void SaveSettings();
    }
}
