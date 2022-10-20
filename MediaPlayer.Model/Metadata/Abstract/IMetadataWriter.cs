using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.Metadata.Abstract
{
    public interface IMetadataWriter
    {
        MetadataLibraries MetadataLibrary { get; }

        void Save(MediaItem mediaItem);
    }
}
