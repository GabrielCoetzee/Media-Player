using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using MediaPlayer.Helpers.Extension_Methods;
using MediaPlayer.MetadataReaders.Interfaces;
using MediaPlayer.Objects;
using TagLib;
using File = TagLib.File;


namespace MediaPlayer.MetadataReaders.Interface_Implementations
{
    public class TaglibMp3MetadataReaderWrapper : IReadMp3Metadata
    {
        #region Properties

        private File _taglibMp3MetadataReader;

        #endregion

        public AudioItem GetMp3Metadata(int id, string path)
        {
            var audioItem = new AudioItem();

            _taglibMp3MetadataReader = File.Create(path);

            audioItem.Id = id;
            audioItem.AlbumArt = _taglibMp3MetadataReader.Tag.Pictures.Length >= 1 ? _taglibMp3MetadataReader.Tag.Pictures[0].Data.Data : audioItem.GetAlbumArtFromDirectory(path);
            audioItem.Album = _taglibMp3MetadataReader.Tag.Album;
            audioItem.Artist = _taglibMp3MetadataReader.Tag.FirstPerformer;
            audioItem.Genre = _taglibMp3MetadataReader.Tag.FirstGenre;
            audioItem.Comments = _taglibMp3MetadataReader.Tag.Comment;
            audioItem.TrackNumber = _taglibMp3MetadataReader.Tag.Track;
            audioItem.Year = _taglibMp3MetadataReader.Tag.Year;
            audioItem.Lyrics = _taglibMp3MetadataReader.Tag.Lyrics;
            audioItem.Composer = _taglibMp3MetadataReader.Tag.FirstComposer;
            audioItem.SongTitle = _taglibMp3MetadataReader.Tag.Title;
            audioItem.MediaDuration = _taglibMp3MetadataReader.Properties.Duration;
            audioItem.Bitrate = _taglibMp3MetadataReader.Properties.AudioBitrate;

            audioItem.HasLyrics = !string.IsNullOrEmpty(audioItem.Lyrics);
            audioItem.FilePath = new Uri(path);

            audioItem.SetWindowTitle();

            Dispose();

            return audioItem;
        }

        public void Dispose()
        {
            _taglibMp3MetadataReader.Dispose();
        }


    }
}
