using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Cleaners.Abstract
{
    public interface IMetadataCleaner
    {
        void Clean(MediaItem mediaItem);
    }
}
