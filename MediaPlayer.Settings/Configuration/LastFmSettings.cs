using Generic.Settings.Abstract;
using Generic.Settings.Concrete;
using System;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.Configuration
{
    [Serializable]
    [InheritedExport]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LastFmSettings : SerializableSettings
    {
        public LastFmSettings()
        {
        }

        [ImportingConstructor]
        public LastFmSettings(IFileLocations fileLocations)
            : base(fileLocations)
        {
            if (!Exists())
                Save();

            CopyToThis(DeserializeObject<LastFmSettings>());
        }

        public string Api { get; set; } = "http://ws.audioscrobbler.com";
        public string ApiKey { get; set; } = "63b8dee9bc3b6d10ff27ae8e5aa58f1d";
        protected override string FileName => @"LastFM Settings";
        protected override bool UseEncryption => true;
        public void Save()
        {
            SerializeObject(this);
        }
    }
}
