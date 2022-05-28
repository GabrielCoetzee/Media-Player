using MediaPlayer.Contracts.Lyrics_OVH;
using System.Threading.Tasks;

namespace MediaPlayer.DataAccess.Abstract
{
    public interface ILyricsOvhDataAccess
    {
        Task<LyricsOvhResponse?> GetLyricsAsync(string artist, string track);
    }
}
