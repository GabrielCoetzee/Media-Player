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

        public Mp3 Mp3 { get; set; } = new Mp3() { };

        #endregion

        public Mp3 GetMp3Metadata(int id, string path)
        {
            if (Mp3 != null)
                Mp3 = new Mp3();

            _taglibMp3MetadataReader = File.Create(path);

            Mp3.Id = id;
            Mp3.AlbumArt = _taglibMp3MetadataReader.Tag.Pictures.Length >= 1 ? _taglibMp3MetadataReader.Tag.Pictures[0].Data.Data : null;
            Mp3.Album = _taglibMp3MetadataReader.Tag.Album;
            Mp3.Artist = _taglibMp3MetadataReader.Tag.FirstPerformer;
            Mp3.Genre = _taglibMp3MetadataReader.Tag.FirstGenre;
            Mp3.Comments = _taglibMp3MetadataReader.Tag.Comment;
            Mp3.TrackNumber = _taglibMp3MetadataReader.Tag.Track;
            Mp3.Year = _taglibMp3MetadataReader.Tag.Year;
            Mp3.Lyrics = _taglibMp3MetadataReader.Tag.Lyrics;
            Mp3.Composer = _taglibMp3MetadataReader.Tag.FirstComposer;
            Mp3.TrackTitle = _taglibMp3MetadataReader.Tag.Title;
            Mp3.TrackDuration = _taglibMp3MetadataReader.Properties.Duration;
            Mp3.Bitrate = _taglibMp3MetadataReader.Properties.AudioBitrate;

            Mp3.HasLyrics = !string.IsNullOrEmpty(Mp3.Lyrics);
            Mp3.FilePath = new Uri(path);

            Dispose();

            return Mp3;
        }

        public void Dispose()
        {
            _taglibMp3MetadataReader.Dispose();
        }
    }
}
