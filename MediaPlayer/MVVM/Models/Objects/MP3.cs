using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using MediaPlayer.Annotations;

namespace MediaPlayer.Objects
{
    public class Mp3 : INotifyPropertyChanged
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
        private Uri _filePath;
        private string _fileName;
        private byte[] _albumArt;
        private string _album;
        private string _artist;
        private string _trackTitle;
        private string _genre;
        private string _comments;
        private uint? _trackNumber;
        private uint? _year;
        private string _lyrics;
        private bool _hasLyrics;
        private string _composer;
        private bool _isSelected;
        private TimeSpan _trackDuration;
        private int _bitrate;

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
        public Uri FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            } 
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        } 
        public byte[] AlbumArt
        {
            get => _albumArt;
            set
            {
                _albumArt = value;
                OnPropertyChanged(nameof(AlbumArt));
            } 
        }
        public string Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            } 
        }
        public string Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            } 
        }
        public string TrackTitle
        {
            get => _trackTitle ?? _fileName;
            set
            {
                _trackTitle = value;

                OnPropertyChanged(nameof(TrackTitle));
                OnPropertyChanged(nameof(TrackTitleTrimmed));
            }
        }

        public string TrackTitleTrimmed
        {
            get
            {
                int charMaxLength = 32;

                var trackTitleTrimmed = TrackTitle;

                if (TrackTitle.Length > charMaxLength)
                    trackTitleTrimmed = TrackTitle.Substring(0, charMaxLength) + "...";

                return trackTitleTrimmed;
            }
        }

        public string Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                OnPropertyChanged(nameof(Genre));
            } 
        }
        public string Comments
        {
            get => _comments;
            set
            {
                _comments = value;
                OnPropertyChanged(nameof(Comments));
            } 
        }
        public uint? TrackNumber
        {
            get => _trackNumber;
            set
            {
                _trackNumber = value;
                OnPropertyChanged(nameof(TrackNumber));
            } 
        }
        public uint? Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged(nameof(Year));
            } 
        }
        public string Lyrics
        {
            get => _lyrics;
            set
            {
                _lyrics = value;
                OnPropertyChanged(nameof(Lyrics));
            } 
        }

        public bool HasLyrics
        {
            get => _hasLyrics;
            set
            {
                _hasLyrics = value;
                OnPropertyChanged(nameof(HasLyrics));
            }
        }
        public string Composer
        {
            get => _composer;
            set
            {
                _composer = value;
                OnPropertyChanged(nameof(Composer));
            } 
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            } 
        }
        public TimeSpan TrackDuration
        {
            get => _trackDuration;
            set
            {
                _trackDuration = value;
                OnPropertyChanged(nameof(TrackDuration));
            } 
        }
        public int Bitrate
        {
            get => _bitrate;
            set
            {
                _bitrate = value;
                OnPropertyChanged(nameof(Bitrate));
            } 
        }

        #endregion Properties
    }
}
