using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Metadata.Abstract.Moderators;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MediaPlayer.Model.Metadata.Concrete.Moderators
{
    [Export(typeof(IMetadataModerator))]
    public class AlbumArtModerator : IMetadataModerator
    {
        public bool IsValid(MediaItem mediaItem)
        {
            if (mediaItem is not AudioItem audioItem)
                return false;

            if (audioItem.HasAlbumArt)
                return false;

            return true;
        }

        public void FixMetadata(MediaItem mediaItem)
        {
            var audioItem = mediaItem as AudioItem;

            audioItem.AlbumArt = SearchForAlbumArtInDirectory(audioItem.FilePath.LocalPath);
            audioItem.IsAlbumArtDirty = false;
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
