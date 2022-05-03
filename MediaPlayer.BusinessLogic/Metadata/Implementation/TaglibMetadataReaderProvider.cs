using MediaPlayer.Common.Enumerations;
using MediaPlayer.BusinessLogic.Abstract;
using TagLib;
using MediaPlayer.Model.Objects.Base;
using MediaPlayer.Model.ObjectBuilders;

namespace MediaPlayer.BusinessLogic.Implementation
{
    public class TaglibMetadataReaderProvider : IMetadataReaderProvider
    {
        #region Properties
        public MetadataReaders MetadataReader => MetadataReaders.Taglib;

        #endregion

        public MediaItem GetFileMetadata(string path)
        {
            try
            {
                using (var taglibMetadataReader = File.Create(path))
                {
                    var albumArt = taglibMetadataReader.Tag.Pictures.Length >= 1 ? taglibMetadataReader.Tag.Pictures[0].Data.Data : null;

                    switch (taglibMetadataReader.Properties.MediaTypes)
                    {
                        case MediaTypes.Audio:

                            var audioItem = new AudioItemBuilder(path)
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
                                .Build();

                            return audioItem;

                        case MediaTypes.Video | MediaTypes.Audio:

                            var videoItem = new VideoItemBuilder(path)
                                .AsMediaType(MediaType.Video | MediaType.Audio)
                                .WithVideoResolution($"{taglibMetadataReader.Properties.VideoWidth} x {taglibMetadataReader.Properties.VideoHeight}")
                                .WithVideoTitle(taglibMetadataReader.Tag.Title)
                                .WithMediaDuration(taglibMetadataReader.Properties.Duration)
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
