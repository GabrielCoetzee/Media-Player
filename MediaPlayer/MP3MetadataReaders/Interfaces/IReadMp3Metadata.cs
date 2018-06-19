using MediaPlayer.Objects;

namespace MediaPlayer.MetadataReaders.Interfaces
{
    public interface IReadMp3Metadata
    {
        AudioItem GetMp3Metadata(string path);

        void Dispose();
    }
}
