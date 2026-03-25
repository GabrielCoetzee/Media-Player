using Generic.Exceptions;
using Generic.Settings.Abstract;
using System;
using System.ComponentModel.Composition;
using System.IO;

namespace Generic.Settings.Concrete
{
    [Export(typeof(IFileLocations))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public abstract class FileLocations : IFileLocations
    {
        public virtual string ConfigurationDirectory => GetDirectory("Configuration");

        public virtual string ApplicationName => throw new MustOverrideException("Application Name must be overridden in child class");

        private string GetDirectory(string folder)
        {
            var path = Path.Combine(ProgramDataDirectory, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        private string ProgramDataDirectory
        {
            get
            {
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                var path = Path.Combine(programData, ApplicationName);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

    }
}
