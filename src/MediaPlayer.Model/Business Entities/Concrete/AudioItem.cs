using Generic.Extensions;
using MediaPlayer.Model.BusinessEntities.Abstract;
using System.Diagnostics;

namespace MediaPlayer.Model.BusinessEntities.Concrete
{
    [DebuggerDisplay("{Artist} - {MediaTitle}")]
    public class AudioItem : MediaItem
    {
        private byte[] _albumArt;
        private string _album;
        private string _artist;
        private string _genre;
        private string _comments;
        private uint? _year;
        private string _lyrics;
        private string _composer;

        public bool HasLyrics => !string.IsNullOrEmpty(_lyrics);
        public bool HasAlbumArt => !_albumArt.IsNullOrEmpty();

        public byte[] AlbumArt => _albumArt;

        public void DisplayLocalAlbumArt(byte[] albumArt)
        {
            _albumArt = albumArt;

            OnPropertyChanged(nameof(AlbumArt));
            OnPropertyChanged(nameof(HasAlbumArt));
        }

        public void EnrichAlbumArt(byte[] albumArt)
        {
            DisplayLocalAlbumArt(albumArt);
            DirtyProperties.Add(nameof(AlbumArt));
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
        public string Lyrics => _lyrics;

        public void SetLyrics(string lyrics)
        {
            _lyrics = lyrics;

            OnPropertyChanged(nameof(Lyrics));
            OnPropertyChanged(nameof(HasLyrics));
        }

        public void EnrichLyrics(string lyrics)
        {
            SetLyrics(lyrics);
            DirtyProperties.Add(nameof(Lyrics));
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
    }
}
