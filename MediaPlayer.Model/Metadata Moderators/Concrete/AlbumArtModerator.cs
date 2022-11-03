using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Moderators.Abstract;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MediaPlayer.Model.Moderators.Concrete
{
    [Export(typeof(IMetadataModerator))]
    public class AlbumArtModerator : IMetadataModerator
    {
        public void FixMetadata(MediaItem mediaItem)
        {
            if (mediaItem is not AudioItem audioItem || audioItem.HasAlbumArt)
                return;

            audioItem.AlbumArt = SearchForAlbumArtInDirectory(audioItem.FilePath.AbsolutePath);
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
