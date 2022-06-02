using MediaPlayer.Model.ObjectBuilders;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;
using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;
using System.ComponentModel.Composition;
using System;
using MediaPlayer.Model.BusinessEntities.Concrete;
using System.Linq;

namespace MediaPlayer.Model.Metadata.Concrete
{
    [Export(typeof(IMetadataWriter))]
    public class TaglibMetadataWriter : IMetadataWriter
    {
        public MetadataLibraries MetadataLibrary => MetadataLibraries.Taglib;

        public void Update(MediaItem mediaItem)
        {
            try
            {
                using var reader = TagLib.File.Create(mediaItem.FilePath.LocalPath);

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
                mediaItem.IsDirty = false;
            }
            catch (Exception)
            {
            }
        }

        private void UpdateLyrics(File reader, AudioItem audioItem)
        {
            if (!string.IsNullOrEmpty(reader.Tag.Lyrics))
                return;

            reader.Tag.Lyrics = audioItem.Lyrics;
        }

        private void UpdateAlbumArt(File reader, AudioItem audioItem)
        {
            if (reader.Tag.Pictures.Any(x => x.Type == PictureType.FrontCover))
                return;

            reader.Tag.Pictures = new TagLib.IPicture[]
            {
                new TagLib.Id3v2.AttachmentFrame
                {
                    Type = PictureType.FrontCover,
                    Description = "Cover",
                    MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                    Data = audioItem.AlbumArt,
                    TextEncoding = TagLib.StringType.UTF16
                }
            };
        }
    }
}
