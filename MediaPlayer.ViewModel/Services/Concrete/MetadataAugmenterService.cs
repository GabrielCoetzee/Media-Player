using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.ViewModel.Services.Abstract;
using System.Linq;
using System.Net.Http;
using System.Threading;
using LazyCache;
using System.Collections.Concurrent;
using MediaPlayer.Model.Metadata.Abstract.Augmenters;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataAugmenterService))]
    public class MetadataAugmenterService : IMetadataAugmenterService
    {
        readonly IAlbumArtMetadataAugmenter _albumArtMetadataAugmenter;
        readonly ILyricsMetadataAugmenter _lyricsMetadataAugmenter;
        readonly IAppCache _cache;

        [ImportingConstructor]
        public MetadataAugmenterService([Import(ServiceNames.LastFmAlbumArtMetadataAugmenter)] IAlbumArtMetadataAugmenter albumArtMetadataAugmenter,
            [Import(ServiceNames.LyricsOvhMetadataAugmenter)] ILyricsMetadataAugmenter lyricsMetadataAugmenter)
        {
            _albumArtMetadataAugmenter = albumArtMetadataAugmenter;
            _lyricsMetadataAugmenter = lyricsMetadataAugmenter;

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
            var lyricsDictionary = new ConcurrentDictionary<string, string>();

            await Task.Run(async () => {

                try
                {
                    await Parallel.ForEachAsync(audioItems, token, async (audioItem, token) =>
                    {
                        token.ThrowIfCancellationRequested();

                        if (audioItem.HasLyrics)
                            return;

                        var lyrics = await _lyricsMetadataAugmenter.GetLyricsAsync(audioItem.Artist, audioItem.MediaTitle);

                        if (string.IsNullOrEmpty(lyrics))
                            return;

                        lyricsDictionary[audioItem.FileName] = lyrics;
                    });
                }
                catch (TaskCanceledException)
                {
                }

            }, token);

            audioItems.Where(x => !x.HasLyrics).ToList().ForEach(x => x.Lyrics = lyricsDictionary.GetValueOrDefault(x.FileName));
        }

        private async Task UpdateAlbumArtAsync(IEnumerable<AudioItem> audioItems, CancellationToken token)
        {
            var albumArtDictionary = new ConcurrentDictionary<string, byte[]>();

            await Task.Run(async () => {

                try
                {
                    await Parallel.ForEachAsync(audioItems, token, async (audioItem, token) =>
                    {
                        token.ThrowIfCancellationRequested();

                        if (audioItem.HasAlbumArt)
                            return;

                        var url = await _albumArtMetadataAugmenter.GetAlbumArtAsync(audioItem.Artist, audioItem.MediaTitle);

                        if (string.IsNullOrEmpty(url))
                            return;

                        async Task<byte[]> DownloadAlbumArtFunction() => await DownloadAlbumArtFromUrlAsync(url);

                        var albumArt = await _cache.GetOrAddAsync(url, DownloadAlbumArtFunction);

                        if (albumArt == null || albumArt.Length == 0)
                            return;

                        albumArtDictionary[audioItem.FileName] = albumArt;
                    });
                }
                catch (TaskCanceledException)
                {
                }

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