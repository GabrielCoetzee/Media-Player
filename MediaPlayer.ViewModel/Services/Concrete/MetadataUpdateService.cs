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

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataUpdateService))]
    public class MetadataUpdateService : IMetadataUpdateService
    {
        readonly ILastFmDataAccess _lastFmDataAccess;
        readonly ILyricsOvhDataAccess _lyricsOvhDataAccess;

        [ImportingConstructor]
        public MetadataUpdateService(ILastFmDataAccess lastFmDataAccess,
            ILyricsOvhDataAccess lyricsOvhDataAccess)
        {
            _lastFmDataAccess = lastFmDataAccess;
            _lyricsOvhDataAccess = lyricsOvhDataAccess;
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

            audioItem.IsDirty = true;
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
                audioItem.AlbumArt = GetAlbumArtFromDirectoryIfAny(audioItem.FilePath.LocalPath);
                return;
            }

            using var client = new HttpClient();
            using var fileDownloadResponse = await client.GetAsync(url);

            audioItem.AlbumArt = await fileDownloadResponse.Content.ReadAsByteArrayAsync();
            audioItem.IsDirty = true;
        }

        private byte[] GetAlbumArtFromDirectoryIfAny(string path)
        {
            try
            {
                var commonCoverArtFileNames = new string[] { "cover.jpg", "folder.jpg" };

                var coverArtFromFolder = Directory
                    .EnumerateFiles(Path.GetDirectoryName(path), "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x => commonCoverArtFileNames.Any(y => y == x));

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
