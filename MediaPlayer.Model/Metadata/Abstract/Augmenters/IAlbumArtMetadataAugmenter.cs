using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Abstract.Augmenters
{
    public interface IAlbumArtMetadataAugmenter
    {
        Task<string> GetAlbumArtAsync(string artist, string track);
    }
}
