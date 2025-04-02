using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using System.Reflection;

namespace MediaPlayer.ViewModel.Test
{
    public static class TestData
    {
        public static string InputTestFilesPath = $"_Test Files/Input Files";

        public static AudioItem AudioItem1 = new()
        {
            Id = 1,
            Album = "Found in Far Away Places",
            Artist = "August Burns Red",
            MediaTitle = "Majoring in the Minors",
            Lyrics = "Test \n\n Lyrics \n\n Spacing",
            FilePath = new Uri($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/_Test Files/Input Files/06. Majoring in the Minors.mp3")
        };

        public static AudioItem AudioItem2 = new()
        {
            Id = 2,
            Album = "Constellations",
            Artist = "August Burns Red",
            MediaTitle = "Meridian (Remixed)",
            FilePath = new Uri($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/_Test Files/Input Files/09. Meridian (Remixed).mp3")
        };

        public static AudioItem AudioItem3 = new()
        {
            Id = 3,
            Album = "Constellations",
            Artist = "August Burns Red",
            MediaTitle = "Meddler (Remixed)",
            FilePath = new Uri($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/_Test Files/Input Files/11. Meddler (Remixed).mp3")
        };

        public static IEnumerable<MediaItem> MediaItems = new List<MediaItem>()
        {
            AudioItem1,
            AudioItem2,
            AudioItem3
        };
    }
}
