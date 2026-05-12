using Flurl.Http;
using Generic.Cache.Abstract;
using MediaPlayer.Common.Constants;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Updaters;
using Polly;
using Polly.Retry;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Concrete.Updaters
{
    [Export(ServiceNames.LastFmAlbumArtMetadataUpdater, typeof(IAlbumArtMetadataUpdater))]
    public class LastFmAlbumArtMetadataUpdater : IAlbumArtMetadataUpdater
    {
        const string UserAgent = "MediaPlayer (+https://github.com/GabrielCoetzee/Media-Player)";

        static readonly ResiliencePipeline _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<FlurlHttpException>(IsTransient),
                MaxRetryAttempts = 2,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(500),
            })
            .Build();

        private static bool IsTransient(FlurlHttpException ex) => ex.StatusCode is null || ex.StatusCode >= 500 || ex.StatusCode == 408;

        readonly ILastFMApi _lastFmApi;
        readonly IRuntimeCache<byte[]> _cache;

        [ImportingConstructor]
        public LastFmAlbumArtMetadataUpdater(ILastFMApi lastFmApi, IRuntimeCache<byte[]> cache)
        {
            _lastFmApi = lastFmApi;
            _cache = cache;
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
            try
            {
                return await _retryPipeline.ExecuteAsync<byte[]>(
                    async _ => await url.WithHeader("User-Agent", UserAgent).GetBytesAsync());
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }

    }
}
