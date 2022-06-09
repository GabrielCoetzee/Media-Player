using MediaPlayer.Settings.Base.Concrete;
using MediaPlayer.Settings.Generic.Abstract;
using System;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.Configuration
{
    [Serializable]
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MetadataSettings : SerializableSettings
    {
        public MetadataSettings()
        {
        }

        [ImportingConstructor]
        public MetadataSettings(IFileLocations fileLocations)
            : base(fileLocations)
        {
            if (!Exists())
                Save();

            CopyToThis(DeserializeObject<MetadataSettings>());
        }

        public bool IsUpdateMetadataEnabled { get; set; } = true;

        public bool IsSaveMetadataToFileEnabled { get; set; } = true;

        protected override string FileName => @"Metadata Settings";

        protected override bool UseEncryption => false;

        public void Save()
        {
            SerializeObject(this);
        }
    }
}
