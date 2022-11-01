using Integration.LastFM.Contracts;
using System.Threading.Tasks;

namespace MediaPlayer.DataAccess.Abstract
{
    public interface ILastFMApi
    {
        Task<LastFmResponseModel?> GetTrackInfoAsync(string artist, string track);
    }
}
