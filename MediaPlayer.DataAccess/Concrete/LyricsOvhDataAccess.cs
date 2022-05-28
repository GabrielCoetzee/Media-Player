using Flurl;
using Flurl.Http;
using MediaPlayer.Contracts.Lyrics_OVH;
using MediaPlayer.DataAccess.Abstract;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace MediaPlayer.DataAccess.Concrete
{
    [Export(typeof(ILyricsOvhDataAccess))]
    public class LyricsOvhDataAccess : ILyricsOvhDataAccess
    {
        public async Task<LyricsOvhResponse?> GetLyricsAsync(string artist, string track)
        {
            try
            {
                return await "https://api.lyrics.ovh/v1"
                    .AppendPathSegments(artist, track)
                    .GetJsonAsync<LyricsOvhResponse>();
            }
            catch (Exception)
            {
            }

            return null;

        }
    }
}
