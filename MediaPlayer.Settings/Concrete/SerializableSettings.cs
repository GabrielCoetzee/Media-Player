using MediaPlayer.Settings.Abstract;
using System;
using System.ComponentModel.Composition;
using System.IO;
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

        protected SerializableSettings(IFileLocations fileLocations)
        {
            _fileLocations = fileLocations;
        }

        private readonly IFileLocations _fileLocations;

        protected abstract string FileName { get; }

        public bool Exists
        {
            get
            {
                var pathName = GetPathName(FileName);

                return File.Exists(pathName);
            }
        }

        string GetPathName(string fileName)
        {
            return Path.Combine(_fileLocations.ConfigurationDirectory, Path.ChangeExtension(fileName, "xml"));
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

        public void Reload<T>() where T : new()
        {
            CopyToThis(DeSerializeObject<T>());
        }

        public void Reset<T>() where T : new()
        {
            CopyToThis(new T());
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
            var tempName = Path.ChangeExtension(pathName, "tmp");
            var backupName = Path.ChangeExtension(pathName, "bak");

            // Ensure a safe write each time.

            var xmlDocument = new XmlDocument();
            var serializer = new XmlSerializer(serializableObject.GetType());

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, serializableObject);
                stream.Position = 0;

                xmlDocument.Load(stream);
                xmlDocument.Save(tempName);
            }

            // If successful write.  Then rename some files

            try
            {
                if (File.Exists(backupName))
                    File.Delete(backupName);

                if (File.Exists(pathName))
                    File.Move(pathName, backupName);

                File.Move(tempName, pathName);
                File.Delete(backupName);
            }
            catch (Exception)
            {
                // If it fails for any reason, put it back

                if (File.Exists(backupName))
                    File.Move(backupName, pathName);
            }
        }

        /// <summary>
        /// Saves the settings object
        /// </summary>
        public void SaveFile()
        {
            if (this == null)
                return;

            var pathName = GetPathName(FileName);
            var tempName = Path.ChangeExtension(pathName, "tmp");
            var backupName = Path.ChangeExtension(pathName, "bak");

            // Ensure a safe write each time.

            var xmlDocument = new XmlDocument();
            var serializer = new XmlSerializer(GetType());

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, this);
                stream.Position = 0;

                xmlDocument.Load(stream);
                xmlDocument.Save(tempName);
            }

            // If successful write.  Then rename some files

            try
            {
                if (File.Exists(backupName))
                    File.Delete(backupName);

                if (File.Exists(pathName))
                    File.Move(pathName, backupName);

                File.Move(tempName, pathName);
                File.Delete(backupName);
            }
            catch (Exception)
            {
                // If it fails for any reason, put it back

                if (File.Exists(backupName))
                    File.Move(backupName, pathName);
            }
        }

        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>

        public T DeSerializeObject<T>() where T : new()
        {
            var pathName = GetPathName(FileName);

            if (string.IsNullOrEmpty(pathName))
                return new T();

            if (!File.Exists(pathName))
                return new T();

            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(pathName);

                using var read = new StringReader(xmlDocument.OuterXml);
                var serializer = new XmlSerializer(typeof(T));

                using XmlReader reader = new XmlTextReader(read);
                return (T)serializer.Deserialize(reader);
            }
            catch
            {
                return new T();
            }
        }
    }
}
