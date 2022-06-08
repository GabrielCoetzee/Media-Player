using System.ComponentModel;

namespace MediaPlayer.Settings
{
    public interface ISettingsManager : INotifyPropertyChanged
    {
        string Accent { get; set;  }
        decimal Opacity { get; set; }
        void SaveSettings();
    }
}
