using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using MediaPlayer.Annotations;
using TagLib;

namespace MediaPlayer.MVVM.Models.Base_Types
{
    public abstract class MediaItem : INotifyPropertyChanged
    {
        #region Interface Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Fields

        private int _id;

        private TimeSpan _mediaDuration { get; set; }

        private Uri _filePath;

        private uint? _mediaListNumber;

        private MediaType _mediaType;

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


        #endregion
    }

    public enum MediaType
    {
        None = 0,
        Audio = 1,
        Video = 2
    }
}
