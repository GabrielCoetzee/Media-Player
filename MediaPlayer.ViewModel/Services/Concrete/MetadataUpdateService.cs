using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.ViewModel.Services.Abstract;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.Common.Enumerations;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Drawing;
using System;
using System.Threading;
using LazyCache;
using System.Diagnostics;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataUpdateService))]
    public class MetadataUpdateService : IMetadataUpdateService
    {
        readonly ILastFmDataAccess _lastFmDataAccess;
        readonly ILyricsOvhDataAccess _lyricsOvhDataAccess;
        readonly IAppCache _cache;

        [ImportingConstructor]
        public MetadataUpdateService(ILastFmDataAccess lastFmDataAccess,
            ILyricsOvhDataAccess lyricsOvhDataAccess)
        {
            _lastFmDataAccess = lastFmDataAccess;
            _lyricsOvhDataAccess = lyricsOvhDataAccess;

            _cache = new CachingService();
        }

        public async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var updateAlbumArtTask = UpdateAlbumArtAsync(audioItems, token);
            var updateLyricsTask = UpdateLyricsAsync(audioItems, token);

            await Task.WhenAll(updateAlbumArtTask, updateLyricsTask);
        }

        private async Task UpdateLyricsAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            foreach (var audioItem in audioItems)
            {
                if (token.IsCancellationRequested)
                    return;

                if (audioItem.HasLyrics)
                    continue;

                var response = await _lyricsOvhDataAccess.GetLyricsAsync(audioItem.Artist, audioItem.MediaTitle);

                audioItem.Lyrics = response?.Lyrics;
                audioItem.IsDirty = audioItem.HasLyrics;
            }
        }

        private async Task UpdateAlbumArtAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            foreach (var audioItem in audioItems)
            {
                if (token.IsCancellationRequested)
                    return;

                if (audioItem.HasAlbumArt)
                    continue;

                var response = await _lastFmDataAccess.GetTrackInfoAsync(audioItem.Artist, audioItem.MediaTitle);

                var url = response?.Track?.Album?.Image?.LastOrDefault()?.Url;

                if (string.IsNullOrEmpty(url))
                    continue;

                async Task<byte[]> DownloadAlbumArtFunction() => await DownloadAlbumArtFromUrlAsync(url);

                audioItem.AlbumArt = await _cache.GetOrAddAsync(url, DownloadAlbumArtFunction);
                audioItem.IsDirty = audioItem.HasAlbumArt;
            }
        }
        private async Task<byte[]> DownloadAlbumArtFromUrlAsync(string url)
        {
            using var client = new HttpClient();
            using var fileDownloadResponse = await client.GetAsync(url);

            return await fileDownloadResponse.Content.ReadAsByteArrayAsync();
        }
    }
}
