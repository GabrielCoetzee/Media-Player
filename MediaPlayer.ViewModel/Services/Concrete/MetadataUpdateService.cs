using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.DataAccess.Abstract;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Linq;
using System.Net.Http;
using System.Threading;
using LazyCache;
using System.Collections.Concurrent;

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
            try
            {
                var updateAlbumArtTask = UpdateAlbumArtAsync(audioItems, token);
                var updateLyricsTask = UpdateLyricsAsync(audioItems, token);

                await Task.WhenAll(updateAlbumArtTask, updateLyricsTask);
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task UpdateLyricsAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var lyricsDictionary = new ConcurrentDictionary<string, string>();

            await Task.Run(async () => {

                await Parallel.ForEachAsync(audioItems, token, async (audioItem, token) =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (audioItem.HasLyrics)
                        return;

                    var response = await _lyricsOvhDataAccess.GetLyricsAsync(audioItem.Artist, audioItem.MediaTitle);
                    var lyrics = response?.Lyrics;

                    if (string.IsNullOrEmpty(lyrics))
                        return;

                    lyricsDictionary[audioItem.FileName] = lyrics;
                });

            }, token);

            audioItems.Where(x => !x.HasLyrics).ToList().ForEach(x => x.Lyrics = lyricsDictionary.GetValueOrDefault(x.FileName));
        }

        private async Task UpdateAlbumArtAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var albumArtDictionary = new ConcurrentDictionary<string, byte[]>();

            await Task.Run(async () => {

                await Parallel.ForEachAsync(audioItems, token, async (audioItem, token) =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (audioItem.HasAlbumArt)
                        return;

                    var response = await _lastFmDataAccess.GetTrackInfoAsync(audioItem.Artist, audioItem.MediaTitle);
                    var url = response?.Track?.Album?.Image?.LastOrDefault()?.Url;

                    if (string.IsNullOrEmpty(url))
                        return;

                    async Task<byte[]> DownloadAlbumArtFunction() => await DownloadAlbumArtFromUrlAsync(url);

                    var albumArt = await _cache.GetOrAddAsync(url, DownloadAlbumArtFunction);

                    if (albumArt == null || albumArt.Length == 0)
                        return;

                    albumArtDictionary[audioItem.FileName] = albumArt;
                });

            }, token);

            audioItems.Where(x => !x.HasAlbumArt).ToList().ForEach(x => x.AlbumArt = albumArtDictionary.GetValueOrDefault(x.FileName));
        }

        private async Task<byte[]> DownloadAlbumArtFromUrlAsync(string url)
        {
            using var client = new HttpClient();
            using var fileDownloadResponse = await client.GetAsync(url);

            return await fileDownloadResponse.Content.ReadAsByteArrayAsync();
        }
    }
}
