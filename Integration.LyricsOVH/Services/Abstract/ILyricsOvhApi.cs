using Integration.LyricsOVH.Contracts;
using System.Threading.Tasks;

namespace Integration.LyricsOVH.Services.Abstract
{
    public interface ILyricsOvhApi
    {
        Task<LyricsOvhResponse?> GetLyricsAsync(string artist, string track);
    }
}
