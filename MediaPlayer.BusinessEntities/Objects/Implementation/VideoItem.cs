using MediaPlayer.BusinessEntities.Objects.Base;

namespace MediaPlayer.BusinessEntities.Objects.Derived
{
    public class VideoItem : MediaItem
    {
        #region Fields

        private string _windowTitle;
        private string _videoTitle;
        private string _videoResolution;
        private string _mediaTitle;

        #endregion

        #region Properties

        public string VideoResolution
        {
            get => _videoResolution;
            set
            {
                _videoResolution = value;
                OnPropertyChanged(nameof(VideoResolution));
            } 
        }

        public string VideoTitle
        {
            get => _videoTitle;
            set
            {
                _videoTitle = value;

                OnPropertyChanged(nameof(VideoTitle));
                OnPropertyChanged(nameof(MediaTitle));
            }
        }

        public override string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public override string MediaTitle
        {
            get => _mediaTitle;
            set
            {
                _mediaTitle = value;
                OnPropertyChanged(nameof(MediaTitle));
            }
        } 

        #endregion

    }
}
