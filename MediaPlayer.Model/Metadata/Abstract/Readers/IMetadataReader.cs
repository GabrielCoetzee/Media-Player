using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.Metadata.Abstract.Readers
{
    public interface IMetadataReader
    {
        MetadataLibraries MetadataLibrary { get; }

        MediaItem BuildMediaItem(string path);
    }
}
