using MediaPlayer.Model.ObjectBuilders;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;
using MediaPlayer.Model.BusinessEntities.Abstract;
using TagLib;
using System.ComponentModel.Composition;
using System;
using MediaPlayer.Model.BusinessEntities.Concrete;

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
                using (var reader = TagLib.File.Create(mediaItem.FilePath.LocalPath))
                {
                    switch (mediaItem.MediaType)
                    {
                        case MediaType.Audio:
                            reader.Tag.Lyrics = (mediaItem as AudioItem).Lyrics;
                            reader.Tag.Pictures = new TagLib.IPicture[] 
                            { 
                                new TagLib.Id3v2.AttachmentFrame 
                                {   
                                    Type = PictureType.FrontCover,
                                    Description = "Cover",
                                    MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                                    Data = (mediaItem as AudioItem).AlbumArt,
                                    TextEncoding = TagLib.StringType.UTF16
                                } 
                            };
                            break;

                        case MediaType.Audio | MediaType.Video:
                            break;
                    }

                    reader.Save();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
