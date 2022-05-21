using MediaPlayer.Settings.Abstract;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace MediaPlayer.Settings.Concrete
{
    [InheritedExport]
    public abstract class SerializableSettings
    {
        protected SerializableSettings()
        {
        }

        [ImportingConstructor]
        protected SerializableSettings(IFileLocations fileLocations)
        {
            _fileLocations = fileLocations;
        }

        private readonly IFileLocations _fileLocations;

        protected abstract string FileName { get; }

        public bool Exists()
        {
            var pathName = GetPathName(FileName);

            return File.Exists(pathName);
        }

        string GetPathName(string fileName)
        {
            return Path.Combine(_fileLocations.ConfigurationDirectory, Path.ChangeExtension(fileName, "json"));
        }

        /// <summary>
        /// Copy all of the serialized properties into the settings class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>

        protected void CopyToThis<T>(T settings)
        {
            foreach (var property in settings.GetType().GetProperties())
            {
                if (!property.CanWrite)
                    continue;

                GetType().GetProperty(property.Name)?.SetValue(this, property.GetValue(settings, null), null);
            }
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>

        public void SerializeObject<T>(T serializableObject) where T : new()
        {
            if (serializableObject == null)
                return;

            var pathName = GetPathName(FileName);

            var json = JsonSerializer.Serialize(serializableObject, new JsonSerializerOptions() { WriteIndented = true });

            if (File.Exists(FileName))
                File.Delete(pathName);

            File.WriteAllText(pathName, json);

        }

        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>

        public T DeserializeObject<T>() where T : new()
        {
            var pathName = GetPathName(FileName);

            if (string.IsNullOrEmpty(pathName))
                return new T();

            if (!File.Exists(pathName))
                return new T();

            try
            {
                using (Stream stream = new FileStream(pathName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
                }
            }
            catch
            {
                return new T();
            }
        }
    }
}
