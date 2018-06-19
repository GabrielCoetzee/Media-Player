using System.Collections.Specialized;
using System.Linq;
using MediaPlayer.Interfaces;

namespace MediaPlayer.Application_Settings.Interface_Implementations
{
    public class ApplicationSettings : IExposeApplicationSettings
    {
        private ApplicationSettings()
        {
        }

        private static readonly object padlock = new object();
        private static ApplicationSettings _instance;

        public static ApplicationSettings Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                        _instance = new ApplicationSettings();

                    return _instance;
                }
            }
        }

        public string[] SupportedAudioFormats
        {
            get
            {
                var supportedAudioFormats = (StringCollection)Properties.Settings.Default[nameof(SupportedAudioFormats)];

                return supportedAudioFormats.Cast<string>().ToArray<string>();
            }
        }

        public string SelectedTheme => Properties.Settings.Default[nameof(SelectedTheme)].ToString();

        public decimal Opacity => (decimal)Properties.Settings.Default[nameof(Opacity)];
    }
}
