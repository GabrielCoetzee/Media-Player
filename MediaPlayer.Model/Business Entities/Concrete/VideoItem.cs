using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Diagnostics;

namespace MediaPlayer.Model.BusinessEntities.Concrete
{
    [DebuggerDisplay("{MediaTitle}")]
    public class VideoItem : MediaItem
    {
        public override bool IsDirty => throw new System.NotImplementedException();
    }
}
