using MediaPlayer.Model.ObjectBuilders;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;
using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Drawing;
using System;

namespace MediaPlayer.Model.Metadata.Concrete
{
    [Export(typeof(IMetadataReader))]
    public class TaglibMetadataReader : IMetadataReader
    {
        public MetadataLibraries MetadataLibrary => MetadataLibraries.Taglib;

        public MediaItem BuildMediaItem(string path)
        {
            try
            {
                using (var reader = TagLib.File.Create(path))
                {
                    var albumArt = reader.Tag.Pictures.Length >= 1 ? reader.Tag.Pictures[0].Data.Data : null;

                    if (albumArt == null || albumArt.Length == 0)
                        albumArt = GetAlbumArtFromDirectory(path);

                    return reader.Properties.MediaTypes switch
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
                }
            }
            catch (CorruptFileException)
            {
                var audioItem = new AudioItemBuilder(path)
                    .Build();

                return audioItem;
            }
        }

        private byte[] GetAlbumArtFromDirectory(string path)
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
