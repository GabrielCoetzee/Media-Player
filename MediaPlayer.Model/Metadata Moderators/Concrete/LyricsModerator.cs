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
        public bool IsValid(MediaItem mediaItem)
        {
            if (mediaItem is not AudioItem audioItem)
                return false;

            if (!audioItem.HasLyrics)
                return false;

            return true;
        }

        public void FixMetadata(MediaItem mediaItem)
        {
            var audioItem = mediaItem as AudioItem;

            audioItem.Lyrics = audioItem.Lyrics.ReplaceTwoSucceedingNewLinesWithOne();
            audioItem.IsLyricsDirty = false;
        }
    }
}
