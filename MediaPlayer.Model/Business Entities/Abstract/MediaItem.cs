using System;
using System.Diagnostics;
using System.IO;
using Generic.PropertyNotify;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.Model.BusinessEntities.Abstract
{
    [DebuggerDisplay("{MediaTitle}")]
    public abstract class MediaItem : NotifyPropertyChanged
    {
        private int? _id;
        private TimeSpan _duration;
        private Uri _filePath;
        private MediaType _mediaType;
        private TimeSpan _elapsedTime;
        private string _mediaTitle;
        private string _windowTitle = "Now Playing: ";
        private string _labelInfo;

        public bool IsVideo => MediaType == (MediaType.Audio | MediaType.Video);
        public bool IsAudio => MediaType == MediaType.Audio;
        public string FileName => Path.GetFileNameWithoutExtension(FilePath.ToString());

        public int? Id
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

        public MediaType MediaType
        {
            get => _mediaType;
            set
            {
                _mediaType = value;
                OnPropertyChanged(nameof(MediaType));
                OnPropertyChanged(nameof(IsVideo));
                OnPropertyChanged(nameof(IsAudio));
            }
        }

        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                OnPropertyChanged(nameof(ElapsedTime));
            }
        }

        public string InfoLabel
        {
            get => _labelInfo;
            set
            {
                _labelInfo = value;
                OnPropertyChanged(nameof(InfoLabel));
            }
        }

        public abstract bool IsDirty { get; }
    }
}
