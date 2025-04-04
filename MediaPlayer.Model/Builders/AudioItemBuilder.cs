﻿using System;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Concrete;

namespace MediaPlayer.Model.ObjectBuilders
{
    public class AudioItemBuilder
    {
        readonly AudioItem _audioItem;

        public AudioItemBuilder(string filePath)
        {
            _audioItem = new AudioItem
            {
                FilePath = new Uri(filePath)
            };
        }

        public AudioItemBuilder AsMediaType(MediaTypes mediaType)
        {
            _audioItem.MediaType = mediaType;

            return this;
        }

        public AudioItemBuilder WithBitrate(int bitrate)
        {
            _audioItem.InfoLabel = $"Bitrate: {bitrate} kbps";

            return this;
        }

        public AudioItemBuilder WithDuration(TimeSpan mediaDuration)
        {
            _audioItem.Duration = mediaDuration;

            return this;
        }

        public AudioItemBuilder WithSongTitle(string songTitle)
        {
            _audioItem.MediaTitle = songTitle ?? _audioItem.FileName;
            _audioItem.WindowTitle += $"{(!string.IsNullOrEmpty(_audioItem.Artist) ? $"{_audioItem.Artist} - {_audioItem.MediaTitle}" : $"{_audioItem.FileName}")}";

            return this;
        }

        public AudioItemBuilder WithComposer(string composer)
        {
            _audioItem.Composer = composer;

            return this;
        }

        /// <summary>
        /// On initial read, setting "IsDirty" to false so we don't write same data back to it later
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns></returns>
        public AudioItemBuilder WithLyrics(string lyrics)
        {
            _audioItem.Lyrics = lyrics;

            _audioItem.DirtyProperties.Remove(nameof(_audioItem.Lyrics));

            return this;
        }

        public AudioItemBuilder WithYear(uint? year)
        {
            _audioItem.Year = year;

            return this;
        }

        /// <summary>
        /// On initial read, setting "IsDirty" to false so we don't write same data back to it later
        /// </summary>
        /// <param name="albumArt"></param>
        /// <returns></returns>
        public AudioItemBuilder WithAlbumArt(byte[] albumArt)
        {
            _audioItem.AlbumArt = albumArt;
            _audioItem.DirtyProperties.Remove(nameof(_audioItem.AlbumArt));

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
    }
}
