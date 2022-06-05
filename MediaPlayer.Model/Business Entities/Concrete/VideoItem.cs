using MediaPlayer.Model.BusinessEntities.Abstract;

namespace MediaPlayer.Model.BusinessEntities.Concrete
{
    public class VideoItem : MediaItem
    {
        private string _videoResolution;

        public string VideoResolution
        {
            get => _videoResolution;
            set
            {
                _videoResolution = value;
                OnPropertyChanged(nameof(VideoResolution));
            } 
        }

        public override bool IsDirty => throw new System.NotImplementedException();
    }
}
