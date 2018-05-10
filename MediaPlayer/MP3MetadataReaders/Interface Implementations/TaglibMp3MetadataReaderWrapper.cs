using System;
using System.IO;
using System.Windows.Controls;
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

        public AudioItem AudioItem { get; set; } = new AudioItem() { };

        #endregion

        public AudioItem GetMp3Metadata(int id, string path)
        {
            if (AudioItem != null)
                AudioItem = new AudioItem();

            _taglibMp3MetadataReader = File.Create(path);

            AudioItem.Id = id;
            AudioItem.AlbumArt = _taglibMp3MetadataReader.Tag.Pictures.Length >= 1 ? _taglibMp3MetadataReader.Tag.Pictures[0].Data.Data : null;
            AudioItem.Album = _taglibMp3MetadataReader.Tag.Album;
            AudioItem.Artist = _taglibMp3MetadataReader.Tag.FirstPerformer;
            AudioItem.Genre = _taglibMp3MetadataReader.Tag.FirstGenre;
            AudioItem.Comments = _taglibMp3MetadataReader.Tag.Comment;
            AudioItem.TrackNumber = _taglibMp3MetadataReader.Tag.Track;
            AudioItem.Year = _taglibMp3MetadataReader.Tag.Year;
            AudioItem.Lyrics = _taglibMp3MetadataReader.Tag.Lyrics;
            AudioItem.Composer = _taglibMp3MetadataReader.Tag.FirstComposer;
            AudioItem.SongTitle = _taglibMp3MetadataReader.Tag.Title;
            AudioItem.MediaDuration = _taglibMp3MetadataReader.Properties.Duration;
            AudioItem.Bitrate = _taglibMp3MetadataReader.Properties.AudioBitrate;

            AudioItem.HasLyrics = !string.IsNullOrEmpty(AudioItem.Lyrics);
            AudioItem.FilePath = new Uri(path);

            AudioItem.SetWindowTitle();

            Dispose();

            return AudioItem;
        }

        public void Dispose()
        {
            _taglibMp3MetadataReader.Dispose();
        }
    }
}
