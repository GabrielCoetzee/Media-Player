﻿using System;
using MediaPlayer.MetadataReaders.Interfaces;
using MediaPlayer.MVVM.Models.Base_Types;
using MediaPlayer.Objects;
using MediaPlayer.Objects.MediaList.Builders;
using MediaPlayer.Objects.MediaList.Derived;
using TagLib;
using File = TagLib.File;


namespace MediaPlayer.MetadataReaders.Interface_Implementations
{
    public class TaglibMetadataReaderWrapper : IReadMetadata
    {
        #region Fields

        private File _taglibMetadataReader;

        #endregion

        public MediaItem GetFileMetadata(string path)
        {
            try
            {
                _taglibMetadataReader = File.Create(path);

                switch (_taglibMetadataReader.Properties.MediaTypes)
                {
                    case MediaTypes.Audio:

                        var audioItem = new AudioItemBuilder(path)
                            .ForAlbum(_taglibMetadataReader.Tag.Album)
                            .WithAlbumArt(_taglibMetadataReader.Tag.Pictures.Length >= 1
                                ? _taglibMetadataReader.Tag.Pictures[0].Data.Data
                                : null)
                            .WithArtist(_taglibMetadataReader.Tag.FirstPerformer)
                            .WithBitrate(_taglibMetadataReader.Properties.AudioBitrate)
                            .WithComments(_taglibMetadataReader.Tag.Comment)
                            .WithComposer(_taglibMetadataReader.Tag.FirstComposer)
                            .WithGenre(_taglibMetadataReader.Tag.FirstGenre)
                            .WithLyrics(_taglibMetadataReader.Tag.Lyrics)
                            .WithMediaDuration(_taglibMetadataReader.Properties.Duration)
                            .WithSongTitle(_taglibMetadataReader.Tag.Title)
                            .WithYear(_taglibMetadataReader.Tag.Year)
                            .AsMediaListNumber(_taglibMetadataReader.Tag.Track)
                            .AsMediaType(MediaType.Audio)
                            .Build();

                        return audioItem;

                    case MediaTypes.Video | MediaTypes.Audio:

                        var videoItem = new VideoItemBuilder(path)
                            .WithVideoResolution($"{_taglibMetadataReader.Properties.VideoWidth} x {_taglibMetadataReader.Properties.VideoHeight}")
                            .WithVideoTitle(_taglibMetadataReader.Tag.Title)
                            .WithMediaDuration(_taglibMetadataReader.Properties.Duration)
                            .AsMediaType(MediaType.Video | MediaType.Audio)
                            .Build();

                        return videoItem;

                    default:
                        return null;
                }
            }
            catch (CorruptFileException)
            {
                var audioItem = new AudioItemBuilder(path).Build();

                return audioItem;
            }
            finally
            {
                this.Dispose();
            }
        }

        private void Dispose()
        {
            _taglibMetadataReader.Dispose();
        }


    }
}
