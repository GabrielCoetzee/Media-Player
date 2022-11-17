using MediaPlayer.Model.ObjectBuilders;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;
using System.ComponentModel.Composition;
using System;
using MediaPlayer.Model.BusinessEntities.Concrete;
using System.Linq;
using MediaPlayer.Model.Metadata.Abstract.Writers;
using MediaPlayer.Common.Constants;

namespace MediaPlayer.Model.Metadata.Concrete.Writers
{
    [Export(ServiceNames.TaglibMetadataWriter, typeof(IMetadataWriter))]
    public class TaglibMetadataWriter : IMetadataWriter
    {
        public void SaveMediaItem(MediaItem mediaItem)
        {
            try
            {
                using var reader = File.Create(mediaItem.FilePath.LocalPath);

                switch (mediaItem.MediaType)
                {
                    case MediaType.Audio:
                        UpdateLyrics(reader, mediaItem as AudioItem);
                        UpdateAlbumArt(reader, mediaItem as AudioItem);
                        break;

                    case MediaType.Audio | MediaType.Video:
                        break;
                }

                reader.Save();
            }
            catch (Exception)
            {
            }
        }

        private void UpdateLyrics(File reader, AudioItem audioItem)
        {
            if (!audioItem.IsLyricsDirty)
                return;

            reader.Tag.Lyrics = audioItem.Lyrics;
            audioItem.IsLyricsDirty = false;
        }

        private void UpdateAlbumArt(File reader, AudioItem audioItem)
        {
            if (!audioItem.IsAlbumArtDirty)
                return;

            reader.Tag.Pictures = new IPicture[]
            {
                new TagLib.Id3v2.AttachmentFrame
                {
                    Type = PictureType.FrontCover,
                    Description = "Cover",
                    MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                    Data = audioItem.AlbumArt,
                    TextEncoding = StringType.UTF16
                }
            };

            audioItem.IsAlbumArtDirty = false;
        }
    }
}
