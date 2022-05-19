﻿using MediaPlayer.Settings.Abstract;
using MediaPlayer.Settings.Concrete;
using System;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.Config
{
    [Serializable]
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Configuration : SerializableSettings
    {
        public Configuration()
        {
        }

        [ImportingConstructor]
        public Configuration(IFileLocations fileLocations) 
            : base(fileLocations)
        {
            if (!Exists)
                Save();

            CopyToThis(DeSerializeObject<Configuration>());
        }

        public string[] SupportedFileFormats { get; set; } = { ".mp3", ".m4a", ".flac", ".wma" };
        public string Accent { get; set; } = "Blue";
        public decimal Opacity { get; set; } = 0.8m;
        protected override string FileName => @"Configuration";

        public void Save()
        {
            SerializeObject(this);
        }
    }
}
