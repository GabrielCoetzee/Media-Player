using MediaPlayer.Common;

namespace MediaPlayer.MetadataReaders.Interfaces
{
    public interface IReadMetadata
    {
        MediaItem GetFileMetadata(string path);
    }
}
