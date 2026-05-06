using Generic.Settings.Abstract;
using Generic.Settings.Concrete;
using System;
using System.ComponentModel.Composition;
using Wpf.Ui.Controls;

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

        public bool UseDarkMode { get; set; } = true;
        public string Accent { get; set; } = "Blue";
        public WindowBackdropType BackdropType { get; set; } = WindowBackdropType.Acrylic;
        public bool AutoAdjustAccent { get; set; } = false;
        protected override string FileName => @"Theme Settings";
        protected override bool UseEncryption => true;
        public void Save()
        {
            SerializeObject(this);
        }
    }
}
