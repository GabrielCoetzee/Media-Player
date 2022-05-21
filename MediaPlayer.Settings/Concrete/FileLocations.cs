using MediaPlayer.Settings.Abstract;
using System;
using System.ComponentModel.Composition;
using System.IO;

namespace MediaPlayer.Settings.Concrete
{
    [Export(typeof(IFileLocations))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FileLocations : IFileLocations
    {
        public string ConfigurationDirectory => GetDirectory("Configuration");

        private string ProgramDataDirectory
        {
            get
            {
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                var path = Path.Combine(programData, "Media Player");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        private string GetDirectory(string folder)
        {
            var path = Path.Combine(ProgramDataDirectory, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

    }
}
