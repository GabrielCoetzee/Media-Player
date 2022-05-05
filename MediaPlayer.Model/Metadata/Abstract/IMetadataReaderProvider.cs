using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.Metadata.Abstract
{
    public interface IMetadataReaderProvider
    {
        MetadataReaders MetadataReader { get; }

        MediaItem GetFileMetadata(string path);
    }
}
