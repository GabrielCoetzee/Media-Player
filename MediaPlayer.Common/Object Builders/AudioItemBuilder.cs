using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MediaPlayer.BusinessEntities
{
    public class AudioItemBuilder
    {
        #region Fields

        private readonly AudioItem _audioItem;

        #endregion

        #region Constructor

        public AudioItemBuilder(string filePath)
        {
            _audioItem = new AudioItem
            {
                FilePath = new Uri(filePath)
            };
        }

        #endregion

        #region Public Methods

        public AudioItemBuilder AsMediaType(MediaType mediaType)
        {
            _audioItem.MediaType = mediaType;

            return this;
        }

        public AudioItemBuilder WithBitrate(int bitrate)
        {
            _audioItem.Bitrate = bitrate;

            return this;
        }

        public AudioItemBuilder WithMediaDuration(TimeSpan mediaDuration)
        {
            _audioItem.MediaDuration = mediaDuration;

            return this;
        }

        public AudioItemBuilder WithSongTitle(string songTitle)
        {
            _audioItem.SongTitle = songTitle;
            _audioItem.MediaTitle = GetMediaTitle();
            _audioItem.WindowTitle = GetWindowTitle();

            return this;
        }

        public AudioItemBuilder WithComposer(string composer)
        {
            _audioItem.Composer = composer;

            return this;
        }

        public AudioItemBuilder WithLyrics(string lyrics)
        {
            _audioItem.Lyrics = lyrics;
            _audioItem.HasLyrics = !string.IsNullOrEmpty(_audioItem.Lyrics);

            return this;
        }

        public AudioItemBuilder WithYear(uint? year)
        {
            _audioItem.Year = year;

            return this;
        }

        public AudioItemBuilder WithAlbumArt(byte[] albumArt)
        {
            _audioItem.AlbumArt = albumArt ?? GetAlbumArtFromDirectory();

            return this;
        }

        public AudioItemBuilder ForAlbum(string album)
        {
            _audioItem.Album = album;

            return this;
        }

        public AudioItemBuilder WithArtist(string artist)
        {
            _audioItem.Artist = artist;

            return this;
        }

        public AudioItemBuilder WithGenre(string genre)
        {
            _audioItem.Genre = genre;

            return this;
        }

        public AudioItemBuilder WithComments(string comments)
        {
            _audioItem.Comments = comments;

            return this;
        }

        public AudioItemBuilder AsMediaListNumber(uint? mediaListNumber)
        {
            _audioItem.MediaListNumber = mediaListNumber;

            return this;
        }

        public AudioItem Build()
        {
            return _audioItem;
        }

        #endregion

        #region Private Methods

        private string GetMediaTitle()
        {
            return _audioItem.SongTitle ?? _audioItem.FileName;
        }

        private string GetWindowTitle()
        {
            if (!string.IsNullOrEmpty(_audioItem.Artist) && !string.IsNullOrEmpty(_audioItem.MediaTitle))
                return $"Now Playing : {_audioItem.Artist} - {_audioItem.MediaTitle}";
            else
                return $"Now Playing : {_audioItem.FileName}";
        }

        private byte[] GetAlbumArtFromDirectory()
        {
            try
            {
                var albumArtFromDirectory = Directory
                    .EnumerateFiles(Path.GetDirectoryName(_audioItem.FilePath.LocalPath), "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x => x.ToLower().EndsWith("cover.jpg") || x.ToLower().EndsWith("folder.jpg"));

                return albumArtFromDirectory.Count() != 0 ? ConvertPathToByteArray(albumArtFromDirectory.First()) : null;
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

        #endregion

    }
}
