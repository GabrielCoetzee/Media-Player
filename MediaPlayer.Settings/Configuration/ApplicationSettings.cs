using MediaPlayer.Settings.Base.Concrete;
using MediaPlayer.Settings.Generic.Abstract;
using System;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.Config
{
    [Serializable]
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ApplicationSettings : SerializableSettings
    {
        public ApplicationSettings()
        {
        }

        [ImportingConstructor]
        public ApplicationSettings(IFileLocations fileLocations) 
            : base(fileLocations)
        {
            if (!Exists())
                Save();

            CopyToThis(DeserializeObject<ApplicationSettings>());
        }


        protected override bool UseEncryption => true;
        public string[] SupportedFileFormats { get; set; } = { ".mp3", ".m4a", ".flac", ".wma" };
        public string Accent { get; set; } = "Blue";
        public decimal Opacity { get; set; } = 0.8m;
        public LastFMSettings LastFMSettings { get; set; } = new LastFMSettings();
        protected override string FileName => @"Application Settings";

        public void Save()
        {
            SerializeObject(this);
        }
    }

    public class LastFMSettings
    {
        public string Api { get; set; } = "http://ws.audioscrobbler.com";
        public string ApiKey { get; set; } = "63b8dee9bc3b6d10ff27ae8e5aa58f1d";
    }
}
