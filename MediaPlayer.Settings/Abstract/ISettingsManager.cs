using System.ComponentModel;

namespace MediaPlayer.Settings
{
    public interface ISettingsManager : INotifyPropertyChanged
    {
        string SelectedAccent { get; set;  }
        decimal SelectedOpacity { get; set; }
        void SaveSettings();
    }
}
