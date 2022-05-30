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

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataUpdateService))]
    public class MetadataUpdateService : IMetadataUpdateService
    {
        readonly ILastFmDataAccess _lastFmDataAccess;
        readonly ILyricsOvhDataAccess _lyricsOvhDataAccess;
        readonly MetadataWriterFactory _metadataWriterFactory;

        [ImportingConstructor]
        public MetadataUpdateService(ILastFmDataAccess lastFmDataAccess,
            ILyricsOvhDataAccess lyricsOvhDataAccess,
            MetadataWriterFactory metadataWriterFactory)
        {
            _lastFmDataAccess = lastFmDataAccess;
            _lyricsOvhDataAccess = lyricsOvhDataAccess;
            _metadataWriterFactory = metadataWriterFactory;
        }

        public async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems)
        {
            foreach (var audioItem in audioItems)
            {
                await UpdateLyricsAsync(audioItem);
                await UpdateAlbumArtAsync(audioItem);

                if (!audioItem.HasAlbumArt)
                    audioItem.AlbumArt = GetAlbumArtFromDirectory(audioItem.FilePath.LocalPath);
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

        private async Task UpdateAlbumArtAsync(AudioItem audioItem)
        {
            if (audioItem.HasAlbumArt)
                return;

            var response = await _lastFmDataAccess.GetTrackInfoAsync(audioItem.Artist, audioItem.MediaTitle);

            var url = response?.Track?.Album?.Image?.LastOrDefault()?.Url;

            if (string.IsNullOrEmpty(url))
                return;

            using (var client = new HttpClient())
            {
                using (var fileDownloadResponse = await client.GetAsync(url))
                {
                    audioItem.AlbumArt = await fileDownloadResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    audioItem.IsDirty = true;
                }
            }
        }

        private byte[] GetAlbumArtFromDirectory(string path)
        {
            try
            {
                var commonCoverArtFileNames = new string[] { "cover.jpg", "folder.jpg" };

                var potentialCoverArtFromFolder = Directory
                    .EnumerateFiles(Path.GetDirectoryName(path), "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x => commonCoverArtFileNames.Any(y => y == x));

                if (!potentialCoverArtFromFolder.Any())
                    return null;

                //Get Larger Album Art from folder if there's more than one option?
                return ConvertPathToByteArray(potentialCoverArtFromFolder.First());
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }

        }

        private byte[] ConvertPathToByteArray(string filePath)
        {
            try
            {
                return (byte[])new ImageConverter().ConvertTo(Image.FromFile(filePath), typeof(byte[]));
            }
            catch (OutOfMemoryException)
            {
                return null;
            }
        }
    }
}
