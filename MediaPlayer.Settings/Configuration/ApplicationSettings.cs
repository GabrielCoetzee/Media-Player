using MediaPlayer.Settings.Abstract;
using MediaPlayer.Settings.Concrete;
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

        public string[] SupportedFileFormats { get; set; } = { ".mp3", ".m4a", ".flac", ".wma" };
        public string Accent { get; set; } = "Blue";
        public decimal Opacity { get; set; } = 0.8m;
        protected override string FileName => @"Application Settings";

        public void Save()
        {
            SerializeObject(this);
        }
    }
}
