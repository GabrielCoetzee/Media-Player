using Generic.Mediator;
using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Settings.Configuration;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace MediaPlayer.Settings.ViewModels
{
    [Export]
    public class SettingsViewModel : NotifyPropertyChanged
    {
        [Import]
        public MetadataSettings MetadataSettings { get; set; }

        [Import]
        public ThemeViewModel ThemeViewModel { get; set; }

        [Import(CommandNames.LoadAccentOptionsCommand)]
        public ICommand LoadAccentOptionsCommand { get; set; }

        [Import(CommandNames.SaveSettings)]
        public ICommand SaveSettingsCommand { get; set; }

        public bool UpdateMetadata
        {
            get => MetadataSettings.UpdateMetadata;
            set
            {
                MetadataSettings.UpdateMetadata = value;
                OnPropertyChanged(nameof(UpdateMetadata));
            }
        }

        public bool SaveMetadataToFile
        {
            get => MetadataSettings.SaveMetadataToFile;
            set
            {
                MetadataSettings.SaveMetadataToFile = value;
                OnPropertyChanged(nameof(SaveMetadataToFile));
            }
        }

        public void SaveSettings()
        {
            ThemeViewModel.SaveSettings();
            MetadataSettings.Save();

            Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
        }
    }
}
