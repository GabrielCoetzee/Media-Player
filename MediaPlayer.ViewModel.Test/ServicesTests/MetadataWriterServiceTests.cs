using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Metadata.Abstract.Writers;
using MediaPlayer.ViewModel.Services.Concrete;
using Moq;
using NUnit.Framework;
using System.Reflection;

namespace MediaPlayer.ViewModel.Test.ServicesTests
{
    [TestFixture]
    public class MetadataWriterServiceTests
    {
        Mock<IMetadataWriter> _metadataWriterMock;

        [SetUp]
        public void SetUp()
        {
            _metadataWriterMock = new Mock<IMetadataWriter>();
        }

        [Test]
        public async Task WriteChangesToFilesInParallel_LyricsUpdated_WritesToFileAndUpdatesDirtyStatus()
        {
            _metadataWriterMock
                .Setup(x => x.WriteToFile(It.Is<MediaItem>(x => x == TestData.AudioItem1)))
                .Callback((MediaItem mediaItem) =>
                {
                    var audioItem = mediaItem as AudioItem;

                    if (audioItem == null || !audioItem.DirtyProperties.Contains(nameof(audioItem.Lyrics)))
                        return;

                    audioItem.Lyrics = "I am adding some lyrics";
                    audioItem.DirtyProperties.Remove(nameof(audioItem.Lyrics));
                });

            var service = new MetadataWriterService(_metadataWriterMock.Object);

            await service.WriteChangesToFilesInParallel(TestData.AudioItems);

            var audioItem = TestData.AudioItem1;

            Assert.That(audioItem.DirtyProperties, Does.Not.Contain(nameof(audioItem.Lyrics)));
            Assert.That(audioItem.DirtyProperties, Is.Empty);
            Assert.That(audioItem.IsDirty, Is.EqualTo(false));
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
                FilePath = new Uri($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/_Test Files/Input Files/06. Majoring in the Minors.mp3")
            };

            public static IEnumerable<AudioItem> AudioItems = new List<AudioItem>() 
            { 
                AudioItem1 
            };
        }
    }
}
