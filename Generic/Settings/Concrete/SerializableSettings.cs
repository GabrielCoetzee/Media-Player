using Generic.Extensions;
using Generic.Settings.Abstract;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.Json;

namespace Generic.Settings.Concrete
{
    [Export]
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
        protected abstract bool UseEncryption { get; }

        public bool Exists()
        {
            var pathName = GetPathName(FileName);

            return File.Exists(pathName);
        }

        string GetPathName(string fileName)
        {
            var ext = UseEncryption ? "enc" : "json";

            return Path.Combine(_fileLocations.ConfigurationDirectory, Path.ChangeExtension(fileName, ext));
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

            if (UseEncryption)
                json = json.Encrypt();

            if (File.Exists(pathName))
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
                var json = File.ReadAllText(pathName);

                if (UseEncryption)
                    json = json.Decrypt();

                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return new T();
            }
        }
    }
}
