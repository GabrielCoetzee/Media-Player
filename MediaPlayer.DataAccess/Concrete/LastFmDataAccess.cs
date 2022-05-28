using Flurl;
using Flurl.Http;
using MediaPlayer.Contracts;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.Settings.Config;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace MediaPlayer.DataAccess.Concrete
{
    [Export(typeof(ILastFmDataAccess))]
    public class LastFmDataAccess : ILastFmDataAccess
    {
        readonly ApplicationSettings _applicationSettings;

        [ImportingConstructor]
        public LastFmDataAccess(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public async Task<LastFmResponseModel?> GetTrackInfoAsync(string artist, string track)
        {
            try
            {
                return await _applicationSettings.LastFMSettings.Api
                    .AppendPathSegments("2.0")
                    .SetQueryParam("method", "track.getinfo")
                    .SetQueryParam("api_key", _applicationSettings.LastFMSettings.ApiKey)
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
