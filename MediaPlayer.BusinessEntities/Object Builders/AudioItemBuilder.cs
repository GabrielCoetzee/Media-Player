﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using MediaPlayer.BusinessEntities.Objects.Derived;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.BusinessEntities.Object_Builders
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

        public AudioItemBuilder WithDuration(TimeSpan mediaDuration)
        {
            _audioItem.Duration = mediaDuration;

            return this;
        }

        public AudioItemBuilder WithSongTitle(string songTitle)
        {
            _audioItem.SongTitle = songTitle;
            _audioItem.MediaTitle = _audioItem.SongTitle ?? _audioItem.FileName;
            _audioItem.WindowTitle += $"{(!string.IsNullOrEmpty(_audioItem.Artist) ? $"{_audioItem.Artist} - {_audioItem.MediaTitle}" : $"{_audioItem.FileName}")}";

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

        public AudioItem Build()
        {
            return _audioItem;
        }

        #endregion

        #region Private Methods

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
