using MediaPlayer.Objects;

namespace MediaPlayer.MetadataReaders.Interfaces
{
    public interface IReadMp3Metadata
    {
        Mp3 GetMp3Metadata(int id, string path);

        void Dispose();
    }
}
