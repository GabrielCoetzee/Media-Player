using MediaPlayer.Model.Metadata.Abstract.Readers;
using MediaPlayer.Settings.Config;
using MediaPlayer.ViewModel.Services.Concrete;
using Moq;
using NUnit.Framework;

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
                _applicationSettings);

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
                _applicationSettings);

            var mediaItems = await service.ReadFilePathsAsync(filepaths);

            Assert.Multiple(() =>
            {
                Assert.That(mediaItems, Is.Not.Null);
                Assert.That(mediaItems, Is.EqualTo(TestData.MediaItems));
            });
        }

        
    }
}
