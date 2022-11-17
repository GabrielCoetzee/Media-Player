using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Abstract.Augmenters
{
    public interface ILyricsMetadataAugmenter
    {
        Task<string> GetLyricsAsync(string artist, string track); 
    }
}
