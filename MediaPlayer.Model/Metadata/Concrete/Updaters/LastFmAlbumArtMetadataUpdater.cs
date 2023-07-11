using Flurl.Http;
using LazyCache;
using MediaPlayer.Common.Constants;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Updaters;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Concrete.Updaters
{
    [Export(ServiceNames.LastFmAlbumArtMetadataUpdater, typeof(IAlbumArtMetadataUpdater))]
    public class LastFmAlbumArtMetadataUpdater : IAlbumArtMetadataUpdater
    {
        readonly ILastFMApi _lastFmApi;
        readonly IAppCache _cache;

        [ImportingConstructor]
        public LastFmAlbumArtMetadataUpdater(ILastFMApi lastFmApi)
        {
            _lastFmApi = lastFmApi;

            _cache = new CachingService();
        }

        public async Task<byte[]> GetAlbumArtAsync(string artist, string track)
        {
            var response = await _lastFmApi.GetTrackInfoAsync(artist, track);

            var url = response?.Track?.Album?.Image?.LastOrDefault()?.Url;

            if (string.IsNullOrEmpty(url))
                return null;

            async Task<byte[]> DownloadAlbumArtFunction() => await DownloadAlbumArtFromUrlAsync(url);

            return await _cache.GetOrAddAsync(url, DownloadAlbumArtFunction);
        }

        private static async Task<byte[]> DownloadAlbumArtFromUrlAsync(string url)
        {
            return await url.GetBytesAsync();
        }
    }
}
