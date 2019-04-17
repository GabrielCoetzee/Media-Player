using System;
using MediaPlayer.BusinessEntities.Objects.Base;

namespace MediaPlayer.MetadataReaders.Abstract
{
    public abstract class MetadataReaderProvider
    {
        public abstract Common.Enumerations.MetadataReaders MetadataReader { get; }
        public virtual MediaItem GetFileMetadata(string path)
        {
            throw new NotSupportedException($"There is no base implementation to read metadata. Please override and provide your own implementation to read metadata");
        }
    }
}
