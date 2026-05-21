using MediaPlayer.Common.Exceptions;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Settings.Configuration;
using MediaPlayer.ViewModel.Services.Abstract;
using MediaPlayer.ViewModel.ViewModels;
using Moq;
using NUnit.Framework;

namespace MediaPlayer.ViewModel.Test.ViewModelTests
{
    [TestFixture]
    public class QueueViewModelTests
    {
        Mock<IMetadataServices> _metadataServicesMock;
        Mock<IMediaLoader> _mediaLoaderMock;
        Mock<IMetadataUpdateService> _metadataUpdaterMock;
        Mock<IMetadataCorrectorService> _metadataCorrectorMock;
        QueueViewModel _vm;

        [SetUp]
        public void SetUp()
        {
            _metadataServicesMock = new Mock<IMetadataServices>();
            _mediaLoaderMock = new Mock<IMediaLoader>();
            _metadataUpdaterMock = new Mock<IMetadataUpdateService>();
            _metadataCorrectorMock = new Mock<IMetadataCorrectorService>();

            _vm = new QueueViewModel
            {
                BusyViewModel = new BusyViewModel(),
                SettingsViewModel = new SettingsViewModel(new MetadataSettings(), themeViewModel: null),
                MediaLoader = _mediaLoaderMock.Object
            };
            _vm.MediaItems.AddRange(TestData.MediaItems.OrderBy(x => x.Id));
        }

        [Test]
        public async Task AddMediaAsync_FilePathsPassedWithoutMetadataUpdate_AddsMediaItemsToListView()
        {
            _mediaLoaderMock
                .Setup(x => x.LoadInBatchesAsync(TestData.FilePaths))
                .Returns(ToBatchedAsyncEnumerable(TestData.MediaItems));

            _vm.MetadataServices = _metadataServicesMock.Object;

            _vm.SettingsViewModel.MetadataSettings.UpdateMetadata = false;

            await _vm.AddMediaAsync(TestData.FilePaths);

            Assert.Multiple(() =>
            {
                Assert.That(_vm.MediaItems, Is.Not.Empty);
                Assert.That(_vm.SelectedMediaItem, Is.EqualTo(TestData.AudioItem1));
            });
        }

        [Test]
        public async Task AddMediaAsync_FilePathsPassedWithMetadataUpdate_AddsMediaItemsToListViewAndUpdatesMetadata()
        {
            _mediaLoaderMock
                .Setup(x => x.LoadInBatchesAsync(TestData.FilePaths))
                .Returns(ToBatchedAsyncEnumerable(TestData.MediaItems));

            _metadataUpdaterMock
                .Setup(x => x.UpdateMetadataAsync(TestData.MediaItems.OfType<AudioItem>()))
                .Callback((IEnumerable<AudioItem> audioItems) => {

                    audioItems.ToList().ForEach(x => x.EnrichAlbumArt(new byte[5] { 2, 4, 6, 8, 10 }));

                });

            _metadataServicesMock.SetupProperty(x => x.MetadataUpdater, _metadataUpdaterMock.Object);
            _metadataServicesMock.SetupProperty(x => x.MetadataCorrector, _metadataCorrectorMock.Object);

            _vm.MetadataServices = _metadataServicesMock.Object;

            _vm.SettingsViewModel.MetadataSettings.UpdateMetadata = true;

            await _vm.AddMediaAsync(TestData.FilePaths);

            Assert.Multiple(() =>
            {
                Assert.That(_vm.MediaItems, Is.Not.Empty);
                Assert.That(_vm.MediaItems.OfType<AudioItem>().All(x => x.HasAlbumArt), Is.EqualTo(true));
            });
        }

        [Test]
        public async Task AddMediaAsync_EmptyFilePathsListPassed_EmptyMediaList()
        {
            _vm.MediaItems.Clear();

            await _vm.AddMediaAsync(new List<string>());

            Assert.That(_vm.MediaItems, Is.Empty);
        }

        [Test]
        public void AddMediaAsync_LoadCancelledMidStream_SwallowsCancellationInsteadOfThrowing()
        {
            _mediaLoaderMock
                .Setup(x => x.LoadInBatchesAsync(It.IsAny<IEnumerable<string>>()))
                .Returns(CancelledMidStream());

            _vm.MetadataServices = _metadataServicesMock.Object;

            Assert.That(async () => await _vm.AddMediaAsync(TestData.FilePaths), Throws.Nothing);
        }

        [Test]
        public void SelectMediaItem_EmptyMediaList_ThrowsEmptyMediaListException()
        {
            _vm.MediaItems.Clear();

            Assert.That(() => _vm.SelectMediaItem(1), Throws.Exception.TypeOf<EmptyMediaListException>());
        }

        [Test]
        public void SelectMediaItem_ValidSelection_ChangesSelectedMediaItemToCorrectIndex()
        {
            var expectedMediaItem = TestData.AudioItem2;
            var index = TestData.MediaItems.IndexOf(expectedMediaItem);

            _vm.SelectMediaItem(index);

            Assert.That(_vm.SelectedMediaItem, Is.EqualTo(expectedMediaItem));
        }

        [Test]
        public void IsPreviousMediaItemAvailable_EmptyMediaList_ReturnsFalse()
        {
            _vm.MediaItems.Clear();

            Assert.That(_vm.IsPreviousMediaItemAvailable(), Is.EqualTo(false));
        }

        [Test]
        public void IsPreviousMediaItemAvailable_PreviousMediaItemIsAvailable_ReturnsTrue()
        {
            _vm.SelectMediaItem(_vm.GetLastMediaItemIndex());

            Assert.That(_vm.IsPreviousMediaItemAvailable(), Is.EqualTo(true));
        }

        [Test]
        public void IsNextMediaItemAvailable_EmptyMediaList_ReturnsFalse()
        {
            _vm.MediaItems.Clear();

            Assert.That(_vm.IsNextMediaItemAvailable(), Is.EqualTo(false));
        }

        [Test]
        public void IsNextMediaItemAvailable_NextMediaItemIsAvailable_ReturnsTrue()
        {
            Assert.That(_vm.IsNextMediaItemAvailable(), Is.EqualTo(true));
        }

        [Test]
        public void GetPreviousMediaItemIndex_PreviousMediaItemIsAvailable_ReturnsPreviousMediaItemIndex()
        {
            _vm.SelectedMediaItem = TestData.AudioItem3;

            var index = TestData.MediaItems.IndexOf(_vm.SelectedMediaItem);

            Assert.That(_vm.GetPreviousMediaItemIndex(), Is.EqualTo(index - 1));
        }

        [Test]
        public void GetNextMediaItemIndex_NextMediaItemIsAvailable_SelectsNextItemInMediaList()
        {
            _vm.SelectedMediaItem = TestData.AudioItem1;

            var index = TestData.MediaItems.IndexOf(_vm.SelectedMediaItem);

            Assert.That(_vm.GetNextMediaItemIndex(), Is.EqualTo(index + 1));
        }

        [Test]
        public void GetFirstMediaItemIndex_WhenCalled_ReturnsIndexOfFirstMediaItemInMediaList()
        {
            Assert.That(_vm.GetFirstMediaItemIndex(), Is.EqualTo(_vm.MediaItems.IndexOf(TestData.AudioItem1)));
        }

        [Test]
        public void GetLastMediaItemIndex_WhenCalled_ReturnsIndexOfLastMediaItemInMediaList()
        {
            Assert.That(_vm.GetLastMediaItemIndex(), Is.EqualTo(_vm.MediaItems.IndexOf(TestData.AudioItem3)));
        }

        [Test]
        public void IsFirstMediaItemSelected_EmptyMediaList_ThrowsEmptyMediaListException()
        {
            _vm.MediaItems.Clear();

            Assert.That(() => _vm.IsFirstMediaItemSelected(), Throws.Exception.TypeOf<EmptyMediaListException>());
        }

        [Test]
        public void IsFirstMediaItemSelected_SelectedMediaItemIsFirstItemInMediaList_ReturnsTrue()
        {
            _vm.SelectMediaItem(_vm.GetFirstMediaItemIndex());

            Assert.That(_vm.IsFirstMediaItemSelected(), Is.EqualTo(true));
        }

        [Test]
        public void IsFirstMediaItemSelected_SelectedMediaItemIsNotFirstItemInMediaList_ReturnsFalse()
        {
            _vm.SelectMediaItem(_vm.GetLastMediaItemIndex());

            Assert.That(_vm.IsFirstMediaItemSelected(), Is.EqualTo(false));
        }

        [Test]
        public void IsLastMediaItemSelected_EmptyMediaList_ThrowsEmptyMediaListException()
        {
            _vm.MediaItems.Clear();

            Assert.That(() => _vm.IsLastMediaItemSelected(), Throws.Exception.TypeOf<EmptyMediaListException>());
        }

        [Test]
        public void IsLastMediaItemSelected_SelectedMediaItemIsLastItemInMediaList_ReturnsTrue()
        {
            _vm.SelectMediaItem(_vm.GetLastMediaItemIndex());

            Assert.That(_vm.IsLastMediaItemSelected(), Is.EqualTo(true));
        }

        [Test]
        public void IsLastMediaItemSelected_SelectedMediaItemIsNotLastItemInList_ReturnsFalse()
        {
            _vm.SelectMediaItem(_vm.GetFirstMediaItemIndex());

            Assert.That(_vm.IsLastMediaItemSelected(), Is.EqualTo(false));
        }

        private static async IAsyncEnumerable<IReadOnlyList<MediaItem>> ToBatchedAsyncEnumerable(IEnumerable<MediaItem> items)
        {
            await Task.CompletedTask;

            yield return items.ToArray();
        }

        private static async IAsyncEnumerable<IReadOnlyList<MediaItem>> CancelledMidStream()
        {
            await Task.CompletedTask;

            yield return new[] { TestData.AudioItem1 };

            throw new OperationCanceledException();
        }

        public static class TestData
        {
            public static IEnumerable<string> FilePaths = new List<string>()
            {
                "Fakedir/Track 1",
                "Fakedir/Track 2",
                "Fakedir/Track 3"
            };

            static TestData()
            {
                AudioItem1.SetLyrics("These are lyrics");
                AudioItem2.SetLyrics("These are lyrics, too");
                AudioItem3.SetLyrics("I am singing, here are some lyrics woo");
            }

            public static AudioItem AudioItem1 = new AudioItem
            {
                Id = 1,
                Album = "Test Album",
                MediaTitle = "Track 1"
            };

            public static AudioItem AudioItem2 = new AudioItem
            {
                Id = 2,
                Album = "Test Album",
                MediaTitle = "Track 2"
            };

            public static AudioItem AudioItem3 = new AudioItem
            {
                Id = 3,
                Album = "Test Album",
                MediaTitle = "Track 3"
            };

            public static List<MediaItem> MediaItems = new List<MediaItem>()
            {
                AudioItem1,
                AudioItem2,
                AudioItem3
            };
        }
    }
}
