using System;
using MediaPlayer.BusinessEntities;

namespace MediaPlayer.MetadataReaders
{
    public abstract class MetadataReaderProvider : IDisposable
    {
        public virtual BusinessEntities.MetadataReaders MetadataReader { get; }
        public virtual MediaItem GetFileMetadata(string path)
        {
            throw new NotSupportedException($"There is no base implementation to read metadata. Please override and provide your own implementation to read metadata");
        }
        public virtual void Dispose()
        {
            throw new NotSupportedException($"There is no base implementation to dispose. Please override and provide your own implementation to read metadata and dispose");
        }
    }
}
