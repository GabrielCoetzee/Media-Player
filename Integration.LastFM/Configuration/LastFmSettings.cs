using System.ComponentModel.Composition;

namespace Integration.LastFM.Configuration
{
    [Export]
    public class LastFmSettings
    {
        public string Api { get; set; } = "http://ws.audioscrobbler.com";
        public string ApiKey { get; set; } = "63b8dee9bc3b6d10ff27ae8e5aa58f1d";
    }
}
