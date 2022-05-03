using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Objects.Base;

namespace MediaPlayer.BusinessLogic.Abstract
{
    public interface IMetadataReaderProvider
    {
        MetadataReaders MetadataReader { get; }

        MediaItem GetFileMetadata(string path);
    }
}
