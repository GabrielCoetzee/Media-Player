using Flurl;
using Flurl.Http;
using Integration.LastFM.Configuration;
using Integration.LastFM.Contracts;
using MediaPlayer.DataAccess.Abstract;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace MediaPlayer.DataAccess.Concrete
{
    [Export(typeof(ILastFMApi))]
    public class LastFMApi : ILastFMApi
    {
        readonly LastFmSettings _lastFmSettings;

        [ImportingConstructor]
        public LastFMApi(LastFmSettings lastFmSettings)
        {
            _lastFmSettings = lastFmSettings;
        }

        public async Task<LastFmResponseModel?> GetTrackInfoAsync(string artist, string track)
        {
            try
            {
                return await _lastFmSettings.Api
                    .AppendPathSegments("2.0")
                    .SetQueryParam("method", "track.getinfo")
                    .SetQueryParam("api_key", _lastFmSettings.ApiKey)
                    .SetQueryParam("artist", artist)
                    .SetQueryParam("track", track)
                    .SetQueryParam("format", "json")
                    .GetJsonAsync<LastFmResponseModel>();
            }
            catch (Exception)
            {
            }

            return null;

        }
    }
}
