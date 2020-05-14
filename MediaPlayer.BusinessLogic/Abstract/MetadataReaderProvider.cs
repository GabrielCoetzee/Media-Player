using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.Common.Enumerations;
using System;

namespace MediaPlayer.BusinessLogic.Abstract
{
    public abstract class MetadataReaderProvider
    {
        public abstract MetadataReaders MetadataReader { get; }
        public virtual MediaItem GetFileMetadata(string path)
        {
            throw new NotSupportedException($"There is no base implementation to read metadata. Please override and provide your own implementation to read metadata");
        }
    }
}
