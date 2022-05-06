using MediaPlayer.Model.ObjectBuilders;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;
using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;

namespace MediaPlayer.Model.Metadata.Concrete
{
    public class TaglibMetadataReaderProvider : IMetadataReaderProvider
    {
        public MetadataReaders MetadataReader => MetadataReaders.Taglib;

        public MediaItem GetFileMetadata(string path)
        {
            try
            {
                using (var taglibMetadataReader = File.Create(path))
                {
                    var albumArt = taglibMetadataReader.Tag.Pictures.Length >= 1 ? taglibMetadataReader.Tag.Pictures[0].Data.Data : null;

                    return taglibMetadataReader.Properties.MediaTypes switch
                    {
                        MediaTypes.Audio => new AudioItemBuilder(path)
                                .AsMediaType(MediaType.Audio)
                                .ForAlbum(taglibMetadataReader.Tag.Album)
                                .WithAlbumArt(albumArt)
                                .WithArtist(taglibMetadataReader.Tag.FirstPerformer)
                                .WithBitrate(taglibMetadataReader.Properties.AudioBitrate)
                                .WithComments(taglibMetadataReader.Tag.Comment)
                                .WithComposer(taglibMetadataReader.Tag.FirstComposer)
                                .WithGenre(taglibMetadataReader.Tag.FirstGenre)
                                .WithLyrics(taglibMetadataReader.Tag.Lyrics)
                                .WithDuration(taglibMetadataReader.Properties.Duration)
                                .WithSongTitle(taglibMetadataReader.Tag.Title)
                                .WithYear(taglibMetadataReader.Tag.Year)
                                .Build(),

                        MediaTypes.Video | MediaTypes.Audio => new VideoItemBuilder(path)
                                .AsMediaType(MediaType.Video | MediaType.Audio)
                                .WithResolution($"{taglibMetadataReader.Properties.VideoWidth} x {taglibMetadataReader.Properties.VideoHeight}")
                                .WithTitle(taglibMetadataReader.Tag.Title)
                                .WithDuration(taglibMetadataReader.Properties.Duration)
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
    }
}
