using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Diagnostics;

namespace MediaPlayer.Model.BusinessEntities.Concrete
{
    [DebuggerDisplay("{MediaTitle}")]
    public class VideoItem : MediaItem
    {
        private string _resolution;

        public string Resolution
        {
            get => _resolution;
            set
            {
                _resolution = value;
                OnPropertyChanged(nameof(Resolution));
            } 
        }

        public override bool IsDirty => throw new System.NotImplementedException();
    }
}
