using Generic.Extensions;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Moderators.Abstract;
using System.ComponentModel.Composition;

namespace MediaPlayer.Model.Cleaners.Concrete
{
    [Export(typeof(IMetadataModerator))]
    public class LyricsModerator : IMetadataModerator
    {
        public void FixMetadata(MediaItem mediaItem)
        {
            if (mediaItem is not AudioItem audioItem || !audioItem.HasLyrics)
                return;

            audioItem.Lyrics = audioItem.Lyrics.ReplaceTwoSucceedingNewLinesWithOne();
            audioItem.IsLyricsDirty = false;
        }
    }
}
