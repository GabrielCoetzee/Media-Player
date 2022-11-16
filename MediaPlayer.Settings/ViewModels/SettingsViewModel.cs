﻿using Generic.PropertyNotify;
using MediaPlayer.Settings.Configuration;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.ViewModels
{
    [Export]
    public class SettingsViewModel : NotifyPropertyChanged
    {
        [Import]
        public MetadataSettings MetadataSettings { get; set; }

        [Import]
        public ThemeViewModel ThemeViewModel { get; set; }

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
        }
    }
}
