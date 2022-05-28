using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.Metadata.Abstract
{
    public interface IMetadataWriter
    {
        MetadataLibraries MetadataLibrary { get; }

        void Update(MediaItem mediaItem);
    }
}
