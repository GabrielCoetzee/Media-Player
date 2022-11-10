using Generic.Settings.Abstract;
using Generic.Settings.Concrete;
using System;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.Config
{
    [Serializable]
    [InheritedExport]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ThemeSettings : SerializableSettings
    {
        public ThemeSettings()
        {
        }

        [ImportingConstructor]
        public ThemeSettings(IFileLocations fileLocations) 
            : base(fileLocations)
        {
            if (!Exists())
                Save();

            CopyToThis(DeserializeObject<ThemeSettings>());
        }

        public string BaseColor { get; set; } = "Dark";
        public string Accent { get; set; } = "Blue";
        public decimal Opacity { get; set; } = 0.8m;
        protected override string FileName => @"Theme Settings";
        protected override bool UseEncryption => true;
        public void Save()
        {
            SerializeObject(this);
        }
    }
}
