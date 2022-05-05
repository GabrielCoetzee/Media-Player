using System;
using System.IO;
using Generic.PropertyNotify;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.Model.BusinessEntities.Abstract
{
    public abstract class MediaItem : PropertyNotifyBase
    {
        private int _id;
        private TimeSpan _duration;
        private Uri _filePath;
        private MediaType _mediaType;
        private TimeSpan _elapsedTime;
        private string _mediaTitle;
        private string _windowTitle = "Now Playing: ";

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Uri FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
                OnPropertyChanged(nameof(FileName));
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public string MediaTitle
        {
            get => _mediaTitle;
            set
            {
                _mediaTitle = value;
                OnPropertyChanged(nameof(MediaTitle));
            }

        }

        public string FileName => Path.GetFileNameWithoutExtension(FilePath.ToString());

        public MediaType MediaType
        {
            get => _mediaType;
            set
            {
                _mediaType = value;
                OnPropertyChanged(nameof(MediaType));
                OnPropertyChanged(nameof(IsVideo));
            }
        }

        public bool IsVideo => MediaType == (MediaType.Audio | MediaType.Video);

        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                OnPropertyChanged(nameof(ElapsedTime));
            }
        }
    }
}
