using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.Metadata.Abstract.Correctors
{
    public interface IMetadataCorrector
    {
        bool IsValid(MediaItem mediaItem);
        void FixMetadata(MediaItem mediaItem);
    }
}
