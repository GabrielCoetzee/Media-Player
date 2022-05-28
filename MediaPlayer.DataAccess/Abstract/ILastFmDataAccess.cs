using MediaPlayer.Contracts;
using System.Threading.Tasks;

namespace MediaPlayer.DataAccess.Abstract
{
    public interface ILastFmDataAccess
    {
        Task<LastFmResponseModel?> GetTrackInfoAsync(string artist, string track);
    }
}
