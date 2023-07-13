using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Metadata.Abstract.Correctors;
using MediaPlayer.Model.Metadata.Abstract.Readers;
using MediaPlayer.Model.Metadata.Concrete.Correctors;
using MediaPlayer.Settings.Config;
using MediaPlayer.ViewModel.Services.Concrete;
using Moq;
using NUnit.Framework;
using System.Reflection;

namespace MediaPlayer.ViewModel.Test.ServicesTests
{
    [TestFixture]
    public class MetadataReaderServiceTests
    {
        ApplicationSettings _applicationSettings;
        Mock<IMetadataReader> _metadataReaderMock;

        [SetUp]
        public void SetUp()
        {
            _metadataReaderMock = new Mock<IMetadataReader>();

            _metadataReaderMock
                .Setup(x => x.BuildMediaItem(It.Is<string>(s => s.Contains(TestData.AudioItem1.MediaTitle))))
                .Returns(TestData.MediaItems.Single(x => x.Id == 1));

            _metadataReaderMock
                .Setup(x => x.BuildMediaItem(It.Is<string>(s => s.Contains(TestData.AudioItem2.MediaTitle))))
                .Returns(TestData.MediaItems.Single(x => x.Id == 2));

            _metadataReaderMock
                 .Setup(x => x.BuildMediaItem(It.Is<string>(s => s.Contains(TestData.AudioItem3.MediaTitle))))
                 .Returns(TestData.MediaItems.Single(x => x.Id == 3));

            _applicationSettings = new ApplicationSettings()
            {
                SupportedFileFormats = new[] { ".mp3", ".m4a", ".flac", ".wma" }
            };
        }

        [Test]
        public async Task ReadFilePathsAsync_ValidFolderpath_BuildsMediaItems()
        {
            var service = new MetadataReaderService(_metadataReaderMock.Object,
                _applicationSettings,
                new List<IMetadataCorrector>() { });

            var mediaItems = await service.ReadFilePathsAsync(new[] { TestData.InputTestFilesPath });

            Assert.Multiple(() =>
            {
                Assert.That(mediaItems, Is.Not.Null);
                Assert.That(mediaItems, Is.EqualTo(TestData.MediaItems));
            });
        }

        [Test]
        public async Task ReadFilePathsAsync_ValidFilepaths_BuildsMediaItems()
        {
            var filepaths = TestData.MediaItems.Select(x => x.FilePath.LocalPath);

            var service = new MetadataReaderService(_metadataReaderMock.Object,
                _applicationSettings,
                new List<IMetadataCorrector>() { });

            var mediaItems = await service.ReadFilePathsAsync(filepaths);

            Assert.Multiple(() =>
            {
                Assert.That(mediaItems, Is.Not.Null);
                Assert.That(mediaItems, Is.EqualTo(TestData.MediaItems));
            });
        }

        [Test]
        public async Task ReadFilePathsAsync_IncludeLyricsCorrector_CorrectorFixesLyricsSpacing()
        {
            var service = new MetadataReaderService(_metadataReaderMock.Object,
                _applicationSettings,
                new List<IMetadataCorrector>() { new LyricsCorrector() });

            var mediaItems = await service.ReadFilePathsAsync(new[] { TestData.InputTestFilesPath });

            var lyrics = (mediaItems.First() as AudioItem)?.Lyrics;

            Assert.That(lyrics, Is.EqualTo($"Test {Environment.NewLine} Lyrics {Environment.NewLine} Spacing"));
        }

        [Test]
        public async Task ReadFilePathsAsync_IncludeAlbumArtCorrector_CorrecterReadsCoverArtFromFolder()
        {
            var service = new MetadataReaderService(_metadataReaderMock.Object,
                _applicationSettings,
                new List<IMetadataCorrector>() { new AlbumArtCorrector() });

            var mediaItems = await service.ReadFilePathsAsync(new[] { TestData.InputTestFilesPath });

            Assert.That(mediaItems.OfType<AudioItem>().All(x => x.HasAlbumArt), Is.EqualTo(true));
        }

        public static class TestData
        {
            public static string InputTestFilesPath = $"_Test Files/Input Files";

            public static AudioItem AudioItem1 = new AudioItem()
            {
                Id = 1,
                Album = "Found in Far Away Places",
                Artist = "August Burns Red",
                MediaTitle = "Majoring in the Minors",
                Lyrics = "Test \n\n Lyrics \n\n Spacing",
                FilePath = new Uri($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/_Test Files/Input Files/06. Majoring in the Minors.mp3")
            };

            public static AudioItem AudioItem2 = new AudioItem()
            {
                Id = 2,
                Album = "Constellations",
                Artist = "August Burns Red",
                MediaTitle = "Meridian (Remixed)",
                FilePath = new Uri($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/_Test Files/Input Files/09. Meridian (Remixed).mp3")
            };

            public static AudioItem AudioItem3 = new AudioItem()
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
}
