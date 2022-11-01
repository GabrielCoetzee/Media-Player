using Flurl;
using Flurl.Http;
using Integration.LyricsOVH.Contracts;
using Integration.LyricsOVH.Services.Abstract;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Integration.LyricsOVH.Services.Concrete
{
    [Export(typeof(ILyricsOvhApi))]
    public class LyricsOvhApi : ILyricsOvhApi
    {
        public async Task<LyricsOvhResponse?> GetLyricsAsync(string artist, string track)
        {
            try
            {
                return await "https://api.lyrics.ovh"
                    .AppendPathSegments("v1")
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
