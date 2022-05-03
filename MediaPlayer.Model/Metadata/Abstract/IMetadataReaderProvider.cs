using MediaPlayer.Model.Objects.Base;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.Model.Abstract
{
    public interface IMetadataReaderProvider
    {
        MetadataReaders MetadataReader { get; }

        MediaItem GetFileMetadata(string path);
    }
}
