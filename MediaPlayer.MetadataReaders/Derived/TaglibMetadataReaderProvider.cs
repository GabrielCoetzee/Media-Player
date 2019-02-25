using MediaPlayer.BusinessEntities.Objects.Abstract;
using MediaPlayer.BusinessEntities.Object_Builders;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.MetadataReaders.Abstract;
using TagLib;

namespace MediaPlayer.MetadataReaders.Derived
{
    public class TaglibMetadataReaderProvider : MetadataReaderProvider
    {
        #region Properties

        public override Common.Enumerations.MetadataReaders MetadataReader => Common.Enumerations.MetadataReaders.Taglib;

        #endregion

        public override MediaItem GetFileMetadata(string path)
        {
            try
            {
                using (var taglibMetadataReader = File.Create(path))
                {
                    switch (taglibMetadataReader.Properties.MediaTypes)
                    {
                        case MediaTypes.Audio:

                            var audioItem = new AudioItemBuilder(path)
                                .ForAlbum(taglibMetadataReader.Tag.Album)
                                .WithAlbumArt(taglibMetadataReader.Tag.Pictures.Length >= 1
                                    ? taglibMetadataReader.Tag.Pictures[0].Data.Data
                                    : null)
                                .WithArtist(taglibMetadataReader.Tag.FirstPerformer)
                                .WithBitrate(taglibMetadataReader.Properties.AudioBitrate)
                                .WithComments(taglibMetadataReader.Tag.Comment)
                                .WithComposer(taglibMetadataReader.Tag.FirstComposer)
                                .WithGenre(taglibMetadataReader.Tag.FirstGenre)
                                .WithLyrics(taglibMetadataReader.Tag.Lyrics)
                                .WithMediaDuration(taglibMetadataReader.Properties.Duration)
                                .WithSongTitle(taglibMetadataReader.Tag.Title)
                                .WithYear(taglibMetadataReader.Tag.Year)
                                .AsMediaListNumber(taglibMetadataReader.Tag.Track)
                                .AsMediaType(MediaType.Audio)
                                .Build();

                            return audioItem;

                        case MediaTypes.Video | MediaTypes.Audio:

                            var videoItem = new VideoItemBuilder(path)
                                .WithVideoResolution($"{taglibMetadataReader.Properties.VideoWidth} x {taglibMetadataReader.Properties.VideoHeight}")
                                .WithVideoTitle(taglibMetadataReader.Tag.Title)
                                .WithMediaDuration(taglibMetadataReader.Properties.Duration)
                                .AsMediaType(MediaType.Video | MediaType.Audio)
                                .Build();

                            return videoItem;

                        default:
                            return null;
                    }
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
