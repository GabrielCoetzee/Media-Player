using System.ComponentModel;

namespace MediaPlayer.ApplicationSettings.Settings_Provider
{
    public interface ISettingsProvider : INotifyPropertyChanged
    {
        string[] SupportedFileFormats { get; }

        string SelectedTheme { get; set; }

        decimal Opacity { get; set; }

        void SaveSettings();
    }
}
