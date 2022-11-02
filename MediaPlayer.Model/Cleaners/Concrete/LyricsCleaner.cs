using Generic.Extensions;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Cleaners.Abstract;
using System.ComponentModel.Composition;

namespace MediaPlayer.Model.Cleaners.Concrete
{
    [Export(typeof(IMetadataCleaner))]
    public class LyricsCleaner : IMetadataCleaner
    {
        public void Clean(MediaItem mediaItem)
        {
            if (mediaItem is not AudioItem audioItem || !audioItem.HasLyrics)
                return;

            audioItem.Lyrics = audioItem.Lyrics.ReplaceTwoSucceedingNewLinesWithOne();
            audioItem.IsLyricsDirty = false;
        }
    }
}
