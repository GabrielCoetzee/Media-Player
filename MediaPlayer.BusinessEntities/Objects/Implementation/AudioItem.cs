using MediaPlayer.BusinessEntities.Objects.Base;

namespace MediaPlayer.BusinessEntities.Objects.Derived
{
    public class AudioItem : MediaItem
    {
        #region Fields

        private byte[] _albumArt;
        private string _album;
        private string _artist;
        private string _songTitle;
        private string _mediaTitle;
        private string _windowTitle;
        private string _genre;
        private string _comments;
        private uint? _year;
        private string _lyrics;
        private bool _hasLyrics;
        private string _composer;
        private int _bitrate;

        #endregion

        #region Overridden Properties

        public override string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public string SongTitle
        {
            get => _songTitle;
            set
            {
                _songTitle = value;

                OnPropertyChanged(nameof(SongTitle));
                OnPropertyChanged(nameof(MediaTitle));
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
    }
}
