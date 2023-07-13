using Generic.Extensions;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Metadata.Abstract.Correctors;
using System.ComponentModel.Composition;

namespace MediaPlayer.Model.Metadata.Concrete.Correctors
{
    [Export(typeof(IMetadataCorrector))]
    public class LyricsCorrector : IMetadataCorrector
    {
        public bool IsValid(MediaItem mediaItem)
        {
            if (mediaItem is not AudioItem audioItem)
                return false;

            return audioItem.HasLyrics;
        }

        public void FixMetadata(MediaItem mediaItem)
        {
            var audioItem = mediaItem as AudioItem;

            audioItem.Lyrics = audioItem.Lyrics.ReplaceTwoSucceedingNewLinesWithOne();
            audioItem.DirtyProperties.Remove(nameof(audioItem.Lyrics));
        }
    }
}
