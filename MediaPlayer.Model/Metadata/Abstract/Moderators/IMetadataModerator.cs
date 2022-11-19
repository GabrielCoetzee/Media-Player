using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.Metadata.Abstract.Moderators
{
    public interface IMetadataModerator
    {
        bool IsValid(MediaItem mediaItem);
        void FixMetadata(MediaItem mediaItem);
    }
}
