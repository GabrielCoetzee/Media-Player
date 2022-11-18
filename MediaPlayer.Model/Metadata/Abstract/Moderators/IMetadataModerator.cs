using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Abstract.Moderators
{
    public interface IMetadataModerator
    {
        bool IsValid(MediaItem mediaItem);
        void FixMetadata(MediaItem mediaItem);
    }
}
