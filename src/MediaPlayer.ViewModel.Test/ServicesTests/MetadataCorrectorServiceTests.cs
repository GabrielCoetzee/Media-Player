using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Metadata.Abstract.Correctors;
using MediaPlayer.Model.Metadata.Concrete.Correctors;
using MediaPlayer.ViewModel.Services.Concrete;
using Moq;
using NUnit.Framework;

namespace MediaPlayer.ViewModel.Test.ServicesTests
{
    [TestFixture]
    public class MetadataCorrectorServiceTests
    {
        [Test]
        public void FixMetadata_LyricsCorrector_CorrectorFixesLyricsSpacing()
        {
            var service = new MetadataCorrectorService(new List<IMetadataCorrector>() { new LyricsCorrector() });

            service.FixMetadata(TestData.MediaItems);

            var lyrics = (TestData.MediaItems.First() as AudioItem)?.Lyrics;

            Assert.That(lyrics, Is.EqualTo($"Test {Environment.NewLine} Lyrics {Environment.NewLine} Spacing"));
        }

        [Test]
        public void FixMetadata_AlbumArtCorrector_CorrecterReadsCoverArtFromFolder()
        {
            var service = new MetadataCorrectorService(new List<IMetadataCorrector>() { new AlbumArtCorrector() });

            service.FixMetadata(TestData.MediaItems);

            Assert.That(TestData.MediaItems.OfType<AudioItem>().All(x => x.HasAlbumArt), Is.EqualTo(true));
        }
    }
}
