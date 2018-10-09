using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MediaPlayer.Annotations;
using MediaPlayer.MVVM.Models.Base_Types;

namespace MediaPlayer.Objects
{
    public class AudioItem : MediaItem, INotifyPropertyChanged
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

        private byte[] _albumArt;
        private string _album;
        private string _artist;
        private string _songTitle;
        private string _genre;
        private string _comments;
        private uint? _year;
        private string _lyrics;
        private bool _hasLyrics;
        private string _composer;
        private int _bitrate;

        #endregion

        #region Overridden Properties

        public override string WindowTitle => SetWindowTitle();

        public string SongTitle
        {
            get => _songTitle;
            set
            {
                _songTitle = value;

                OnPropertyChanged(nameof(SongTitle));
                OnPropertyChanged(nameof(MediaTitle));
                OnPropertyChanged(nameof(MediaTitleTrimmed));
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public override string MediaTitle => _songTitle ?? FileName;

        #endregion

        #region Properties

        public byte[] AlbumArt
        {
            get => _albumArt;
            set
            {
                _albumArt = value;
                OnPropertyChanged(nameof(AlbumArt));
                OnPropertyChanged(nameof(HasAlbumArt));
            }
        }

        public bool HasAlbumArt => _albumArt != null;

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

        #region Internal Methods
        internal byte[] GetAlbumArtFromDirectory(string filePath)
        {
            var albumArtFromDirectory = Directory
                .EnumerateFiles(Path.GetDirectoryName(filePath), "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => x.ToLower().EndsWith("cover.jpg") || x.ToLower().EndsWith("folder.jpg"));

            if (albumArtFromDirectory.Count() != 0)
            {
                return ConvertPathToByteArray(albumArtFromDirectory.First());
            }

            return null;
        }

        #endregion

        #region Private Methods

        public byte[] ConvertPathToByteArray(string filePath)
        {
            try
            {
                return (byte[])new ImageConverter().ConvertTo(Image.FromFile(filePath), typeof(byte[]));
            }
            catch (OutOfMemoryException)
            {
                return null;
            }
        }

        private string SetWindowTitle()
        {
            if (!string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(MediaTitle))
                return $"Now Playing : {Artist} - {MediaTitle}";
            else
                return $"Now Playing : {FileName}";
        }

        #endregion
    }
}
