using System;
using System.ComponentModel;
using System.IO;
using MediaPlayer.Common.Enumerations;

namespace MediaPlayer.BusinessEntities.Objects.Base
{
    public abstract class MediaItem : INotifyPropertyChanged
    {
        #region Interface Implementations

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        #region Fields

        private int _id;
        private TimeSpan _mediaDuration { get; set; }
        private Uri _filePath;
        private uint? _mediaListNumber;
        private MediaType _mediaType;
        private TimeSpan _elapsedTime;

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public uint? MediaListNumber
        {
            get => _mediaListNumber;
            set
            {
                _mediaListNumber = value;
                OnPropertyChanged(nameof(MediaListNumber));
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

        public TimeSpan MediaDuration
        {
            get => _mediaDuration;
            set
            {
                _mediaDuration = value;
                OnPropertyChanged(nameof(MediaDuration));
            }
        }

        public abstract string WindowTitle { get; set; }

        public abstract string MediaTitle { get; set; }

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

        #endregion
    }
}
