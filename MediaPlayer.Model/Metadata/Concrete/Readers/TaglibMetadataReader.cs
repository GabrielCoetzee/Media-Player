using MediaPlayer.Model.ObjectBuilders;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Drawing;
using System;
using System.Collections.Generic;
using MediaPlayer.Model.Metadata.Abstract.Readers;
using MediaPlayer.Model.Moderators.Abstract;

namespace MediaPlayer.Model.Metadata.Concrete.Readers
{
    [Export(typeof(IMetadataReader))]
    public class TaglibMetadataReader : IMetadataReader
    {
        public MetadataLibraries MetadataLibrary => MetadataLibraries.Taglib;

        [ImportMany(typeof(IMetadataModerator))]
        public List<IMetadataModerator> MetadataModerators { get; set; }

        public MediaItem BuildMediaItem(string path)
        {
            try
            {
                using var reader = TagLib.File.Create(path);

                MediaItem mediaItem = reader.Properties.MediaTypes switch
                {
                    MediaTypes.Audio => new AudioItemBuilder(path)
                            .AsMediaType(MediaType.Audio)
                            .ForAlbum(reader.Tag.Album)
                            .WithAlbumArt(reader.Tag.Pictures.FirstOrDefault(x => x.Type == PictureType.FrontCover)?.Data?.Data)
                            .WithArtist(reader.Tag.FirstPerformer)
                            .WithBitrate(reader.Properties.AudioBitrate)
                            .WithComments(reader.Tag.Comment)
                            .WithComposer(reader.Tag.FirstComposer)
                            .WithGenre(reader.Tag.FirstGenre)
                            .WithLyrics(reader.Tag.Lyrics)
                            .WithDuration(reader.Properties.Duration)
                            .WithSongTitle(reader.Tag.Title)
                            .WithYear(reader.Tag.Year)
                            .Build(),

                    MediaTypes.Video | MediaTypes.Audio => new VideoItemBuilder(path)
                            .AsMediaType(MediaType.Video | MediaType.Audio)
                            .WithResolution($"{reader.Properties.VideoWidth} x {reader.Properties.VideoHeight}")
                            .WithTitle(reader.Tag.Title)
                            .WithDuration(reader.Properties.Duration)
                            .Build(),

                    _ => null
                };

                MetadataModerators.ForEach(x => x.FixMetadata(mediaItem));

                return mediaItem;
            }
            catch (CorruptFileException)
            {
                var audioItem = new AudioItemBuilder(path)
                    .Build();

                return audioItem;
            }
        }
    }
}
