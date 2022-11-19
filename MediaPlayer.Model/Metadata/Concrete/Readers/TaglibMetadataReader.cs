using MediaPlayer.Model.ObjectBuilders;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;
using System.ComponentModel.Composition;
using System.Linq;
using MediaPlayer.Model.Metadata.Abstract.Readers;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.Model.Metadata.Concrete.Readers
{
    [Export(ServiceNames.TaglibMetadataReader, typeof(IMetadataReader))]
    public class TaglibMetadataReader : IMetadataReader
    {
        public MediaItem BuildMediaItem(string path)
        {
            try
            {
                using var reader = TagLib.File.Create(path);

                return reader.Properties.MediaTypes switch
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
