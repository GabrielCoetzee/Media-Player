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
using MediaPlayer.Model.Cleaners.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Readers;

namespace MediaPlayer.Model.Metadata.Concrete.Readers
{
    [Export(typeof(IMetadataReader))]
    public class TaglibMetadataReader : IMetadataReader
    {
        public MetadataLibraries MetadataLibrary => MetadataLibraries.Taglib;

        [ImportMany(typeof(IMetadataCleaner))]
        public List<IMetadataCleaner> MetadataCleaners { get; set; }

        public MediaItem BuildMediaItem(string path)
        {
            try
            {
                using var reader = TagLib.File.Create(path);

                var albumArt = reader.Tag.Pictures.FirstOrDefault(x => x.Type == PictureType.FrontCover)?.Data?.Data;

                if (albumArt == null || albumArt.Length == 0)
                    albumArt = SearchForAlbumArtInDirectory(path);

                MediaItem mediaItem = reader.Properties.MediaTypes switch
                {
                    MediaTypes.Audio => new AudioItemBuilder(path)
                            .AsMediaType(MediaType.Audio)
                            .ForAlbum(reader.Tag.Album)
                            .WithAlbumArt(albumArt)
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

                MetadataCleaners.ForEach(x => x.Clean(mediaItem));

                return mediaItem;
            }
            catch (CorruptFileException)
            {
                var audioItem = new AudioItemBuilder(path)
                    .Build();

                return audioItem;
            }
        }

        private byte[] SearchForAlbumArtInDirectory(string path)
        {
            try
            {
                var commonCoverArtFileNames = new string[] { "cover.jpg", "folder.jpg" };

                var coverArtFromFolder = Directory
                    .EnumerateFiles(Path.GetDirectoryName(path), "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x => commonCoverArtFileNames.Contains(Path.GetFileName(x.ToLower())));

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
