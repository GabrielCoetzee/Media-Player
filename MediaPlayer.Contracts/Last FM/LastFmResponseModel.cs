using Newtonsoft.Json;
using System.Text.Json;

namespace MediaPlayer.Contracts
{
    public class LastFmResponseModel
    {
        //public Album? Album { get; set; }

        //public Tracks? Tracks { get; set; }

        public Track? Track { get; set; }

        //public Attr? Attr { get; set; }

        //public Streamable? Streamable { get; set; }

        //public Artist? Artist { get; set; }

        //public Tags? Tags { get; set; }

        //public Tag? Tag { get; set; }

        //public Image? Image { get; set; }
    }

    public class Album
    {
        public string? Name { get; set; }
        public string? Artist { get; set; }
        public string? Mbid { get; set; }
        public string? Url { get; set; }
        public Image[]? Image { get; set; }
        public string? Listeners { get; set; }
        public string? Playcount { get; set; }
        public Tracks? Tracks { get; set; }
        public Tags? Tags { get; set; }
    }

    public class Tracks
    {
        public Track[]? Track { get; set; }
    }

    public class Track
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Duration { get; set; }
        //public Attr? Attr { get; set; }
        //public Streamable? Streamable { get; set; }
        public Artist? Artist { get; set; }
        public Album? Album { get; set; }
    }

    public class Attr
    {
        public string? Rank { get; set; }
    }

    public class Streamable
    {
        public string? Text { get; set; }
        public string? Fulltrack { get; set; }
    }

    public class Artist
    {
        //public Album? Album { get; set; }
        public string? Name { get; set; }
        public string? Mbid { get; set; }
        public string? Url { get; set; }
    }

    public class Tags
    {
        public Tag[]? Tag { get; set; }
    }

    public class Tag
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
    }

    public class Image
    {
        [JsonProperty("#text")]
        public string? Url { get; set; }

        public string? Size { get; set; }
    }
}
