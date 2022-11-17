using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Moderators.Abstract
{
    public interface IMetadataModerator
    {
        bool IsValid(MediaItem mediaItem);
        void FixMetadata(MediaItem mediaItem);
    }
}
