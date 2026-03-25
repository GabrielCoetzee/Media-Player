using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete.Readers.Taglib.Abstract;
using MediaPlayer.Model.ObjectBuilders;
using System.ComponentModel.Composition;
using System.Linq;
using TagLib;

namespace MediaPlayer.Model.Metadata.Concrete.Readers.Taglib.Concrete
{
    [Export(typeof(ITaglibMediaTypeIdentifiable))]
    public class AudioMediaType : ITaglibMediaTypeIdentifiable
    {
        public bool IsValid(MediaTypes mediaType)
        {
            return mediaType == MediaTypes.Audio;
        }

        public MediaItem BuildMediaItem(File reader)
        {
            return new AudioItemBuilder(reader.Name)
                .AsMediaType(Common.Enumerations.MediaTypes.Audio)
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
                .Build();
        }
    }
}
