using System.ComponentModel;

namespace MediaPlayer.ApplicationSettings
{
    public interface ISettingsProvider : INotifyPropertyChanged
    {
        string SelectedAccent { get; set;  }
        decimal SelectedOpacity { get; set; }
        string[] SupportedFileFormats { get; }

        void SaveSettings();
    }
}
