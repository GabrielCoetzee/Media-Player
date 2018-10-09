using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPlayer.MVVM.Models.Base_Types;

namespace MediaPlayer.Objects.MediaList.Derived
{
    public class VideoItem : MediaItem, INotifyPropertyChanged
    {
        #region Fields

        private string _windowTitle;
        private string _videoTitle;
        private int _videoHeight;
        private int _videoWidth;

        #endregion

        #region Properties

        public int VideoHeight
        {
            get => _videoHeight;
            set
            {
                _videoHeight = value;
                OnPropertyChanged(nameof(VideoHeight));
            }
        }

        public int VideoWidth
        {
            get => _videoWidth;
            set
            {
                _videoWidth = value;
                OnPropertyChanged(nameof(VideoWidth));
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
                OnPropertyChanged(nameof(MediaTitleTrimmed));
            }
        }

        public override string WindowTitle => SetWindowTitle();

        public override string MediaTitle => _videoTitle ?? FileName;

        #endregion

        #region Private Methods

        private string SetWindowTitle()
        {
            if (!string.IsNullOrEmpty(MediaTitle) && !string.IsNullOrEmpty(MediaTitle))
                return $"Now Playing : {MediaTitle}";
            else
                return  $"Now Playing : {FileName}";
        }

        #endregion

    }
}
