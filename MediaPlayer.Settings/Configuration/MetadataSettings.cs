using Generic.Settings.Abstract;
using Generic.Settings.Concrete;
using System;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.Configuration
{
    [Serializable]
    [InheritedExport]
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

        public bool UpdateMetadata { get; set; } = true;
        public bool SaveMetadataToFile { get; set; } = false;
        protected override string FileName => @"Metadata Settings";
        protected override bool UseEncryption => false;
        public void Save()
        {
            SerializeObject(this);
        }
    }
}
