using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.Metadata.Abstract.Writers
{
    public interface IMetadataWriter
    {
        void WriteToFile(MediaItem mediaItem);
    }
}
