using MediaPlayer.MVVM.Models.Base_Types;

namespace MediaPlayer.MetadataReaders.Interfaces
{
    public interface IReadMetadata
    {
        MediaItem GetFileMetadata(string path);

        void Dispose();
    }
}
