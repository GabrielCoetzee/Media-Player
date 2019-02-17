using System;
using MediaPlayer.BusinessEntities.Enumerations;
using MediaPlayer.BusinessEntities.Objects.Abstract;

namespace MediaPlayer.Common.Metadata_Readers.Abstract
{
    public abstract class MetadataReaderProvider
    {
        public virtual MetadataReaders MetadataReader { get; }
        public virtual MediaItem GetFileMetadata(string path)
        {
            throw new NotSupportedException($"There is no base implementation to read metadata. Please override and provide your own implementation to read metadata");
        }
    }
}
