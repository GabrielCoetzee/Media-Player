﻿using LazyCache;
using MediaPlayer.Common.Constants;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Augmenters;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Concrete.Augmenters
{
    [Export(ServiceNames.LastFmAlbumArtMetadataAugmenter, typeof(IAlbumArtMetadataAugmenter))]
    public class LastFmAlbumArtMetadataAugmenter : IAlbumArtMetadataAugmenter
    {
        readonly ILastFMApi _lastFmApi;
        readonly IAppCache _cache;

        [ImportingConstructor]
        public LastFmAlbumArtMetadataAugmenter(ILastFMApi lastFmApi)
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
            using var client = new HttpClient();
            using var fileDownloadResponse = await client.GetAsync(url);

            return await fileDownloadResponse.Content.ReadAsByteArrayAsync();
        }
    }
}
