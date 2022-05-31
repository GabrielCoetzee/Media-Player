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
            foreach (var audioItem in audioItems)
            {
                if (token.IsCancellationRequested)
                    return;

                await UpdateLyricsAsync(audioItem);
                await UpdateAlbumArtAsync(audioItem);
            }
        }

        private async Task UpdateLyricsAsync(AudioItem audioItem)
        {
            if (audioItem.HasLyrics)
                return;

            var response = await _lyricsOvhDataAccess.GetLyricsAsync(audioItem.Artist, audioItem.MediaTitle);
            audioItem.Lyrics = response?.Lyrics;
            audioItem.IsDirty = audioItem.HasLyrics;
        }


        /// <summary>
        /// Attempts to get album art from last.fm if there is none. If no album art is found, it will scan for album art
        /// in the current directory and display it, but not save it to file, as there's no guarantee it is correct.
        /// </summary>
        /// <param name="audioItem"></param>
        /// <returns></returns>
        private async Task UpdateAlbumArtAsync(AudioItem audioItem)
        {
            if (audioItem.HasAlbumArt)
                return;

            var response = await _lastFmDataAccess.GetTrackInfoAsync(audioItem.Artist, audioItem.MediaTitle);

            var url = response?.Track?.Album?.Image?.LastOrDefault()?.Url;

            if (string.IsNullOrEmpty(url))
            {
                Func<byte[]> GetAlbumArtFromDirectoryAction = () => GetAlbumArtFromDirectoryIfAny(audioItem.FilePath.LocalPath);

                audioItem.AlbumArt = _cache.GetOrAdd("FolderCoverArt", GetAlbumArtFromDirectoryAction);
                return;
            }

            Func<Task<byte[]>> DownloadAlbumArtAction = async () => await DownloadAlbumArtFromUrlAsync(url);

            audioItem.AlbumArt = await _cache.GetOrAddAsync(url, DownloadAlbumArtAction);
            audioItem.IsDirty = audioItem.HasAlbumArt;
        }

        private async Task<byte[]> DownloadAlbumArtFromUrlAsync(string url)
        {
            using var client = new HttpClient();
            using var fileDownloadResponse = await client.GetAsync(url);

            return await fileDownloadResponse.Content.ReadAsByteArrayAsync();
        }

        private byte[] GetAlbumArtFromDirectoryIfAny(string path)
        {
            try
            {
                var commonCoverArtFileNames = new string[] { "cover.jpg", "folder.jpg" };

                var coverArtFromFolder = Directory
                    .EnumerateFiles(Path.GetDirectoryName(path), "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x => commonCoverArtFileNames.Contains(Path.GetFileName(x)));

                if (!coverArtFromFolder.Any())
                    return null;

                return (byte[])new ImageConverter().ConvertTo(Image.FromFile(coverArtFromFolder.First()), typeof(byte[]));
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
