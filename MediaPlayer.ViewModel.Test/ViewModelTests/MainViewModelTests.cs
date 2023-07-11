using MediaPlayer.Common.Exceptions;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Settings.Configuration;
using MediaPlayer.Settings.ViewModels;
using MediaPlayer.ViewModel.Services.Abstract;
using MediaPlayer.ViewModel.ViewModels;
using Moq;
using NUnit.Framework;
using System.Windows.Controls;

namespace MediaPlayer.ViewModel.Test.ViewModelTests
{
    [TestFixture]
    public class MainViewModelTests
    {
        Mock<IMetadataServices> _metadataServicesMock;
        Mock<IMetadataReaderService> _metadataReaderMock;
        Mock<IMetadataUpdateService> _metadataUpdaterMock;
        MainViewModel _vm;

        [SetUp]
        public void SetUp()
        {
            _metadataServicesMock = new Mock<IMetadataServices>();
            _metadataReaderMock = new Mock<IMetadataReaderService>();
            _metadataUpdaterMock = new Mock<IMetadataUpdateService>();

            _vm = new MainViewModel
            {
                BusyViewModel = new BusyViewModel(),
                MediaControlsViewModel = new MediaControlsViewModel(),
                SettingsViewModel = new SettingsViewModel()
                {
                    MetadataSettings = new MetadataSettings()
                },
                MediaItems = new Model.Collections.MediaItemObservableCollection(TestData.MediaItems.OrderBy(x => x.Id))
            };
        }

        [Test]
        public async Task ProcessFilePathsAsync_FilePathsPassedWithoutMetadataUpdate_AddsMediaItemsToListView()
        {
            _metadataReaderMock
                .Setup(x => x.ReadFilePathsAsync(TestData.FilePaths))
                .ReturnsAsync(TestData.MediaItems);

            _metadataServicesMock.SetupProperty(x => x.MetadataReader, _metadataReaderMock.Object);

            _vm.MetadataServices = _metadataServicesMock.Object;

            _vm.SettingsViewModel.MetadataSettings.UpdateMetadata = false;

            await _vm.ProcessFilePathsAsync(TestData.FilePaths);

            Assert.Multiple(() =>
            {
                Assert.That(_vm.MediaItems, Is.Not.Empty);
                Assert.That(_vm.SelectedMediaItem, Is.EqualTo(TestData.MediaItems.First()));
                Assert.That(_vm.MediaControlsViewModel.MediaState, Is.EqualTo(MediaState.Play));
            });
        }

        [Test]
        public async Task ProcessFilePathsAsync_FilePathsPassedWithMetadataUpdate_AddsMediaItemsToListViewAndUpdatesMetadata()
        {
            _metadataReaderMock
                .Setup(x => x.ReadFilePathsAsync(TestData.FilePaths))
                .ReturnsAsync(TestData.MediaItems);

            _metadataUpdaterMock.Setup(x => x.UpdateMetadataAsync(TestData.MediaItems.OfType<AudioItem>(), It.IsAny<CancellationToken>()))
                   .Callback((IEnumerable<AudioItem> audioItems, CancellationToken token) => {

                       //Mock updating album art
                       audioItems.ToList().ForEach(x => x.AlbumArt = new byte[5] { 2, 4, 6, 8, 10 });
                   });

            _metadataServicesMock.SetupProperty(x => x.MetadataReader, _metadataReaderMock.Object);
            _metadataServicesMock.SetupProperty(x => x.MetadataUpdater, _metadataUpdaterMock.Object);

            _vm.MetadataServices = _metadataServicesMock.Object;

            _vm.SettingsViewModel.MetadataSettings.UpdateMetadata = true;

            await _vm.ProcessFilePathsAsync(TestData.FilePaths);

            Assert.Multiple(() =>
            {
                Assert.That(_vm.MediaItems, Is.Not.Empty);
                Assert.That(_vm.MediaItems.OfType<AudioItem>().All(x => x.HasAlbumArt), Is.EqualTo(true));
            });
        }

        [Test]
        public async Task ProcessFilePathsAsync_EmptyFilePathsListPassed_EmptyMediaList()
        {
            _vm.MediaItems.Clear();

            await _vm.ProcessFilePathsAsync(new List<string>());

            Assert.That(_vm.MediaItems, Is.Empty);
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
            var expectedMediaItem = TestData.MediaItems.Single(x => x.Id == 2);
            var index = TestData.MediaItems.ToList().IndexOf(expectedMediaItem);

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
            _vm.SelectedMediaItem = TestData.MediaItems.Last();

            var index = TestData.MediaItems.ToList().IndexOf(_vm.SelectedMediaItem);

            Assert.That(_vm.GetPreviousMediaItemIndex(), Is.EqualTo(index - 1));
        }

        [Test]
        public void GetNextMediaItemIndex_NextMediaItemIsAvailable_SelectsNextItemInMediaList()
        {
            _vm.SelectedMediaItem = TestData.MediaItems.First();

            var index = TestData.MediaItems.ToList().IndexOf(_vm.SelectedMediaItem);

            Assert.That(_vm.GetNextMediaItemIndex(), Is.EqualTo(index + 1));
        }

        [Test]
        public void GetFirstMediaItemIndex_WhenCalled_ReturnsIndexOfFirstMediaItemInMediaList()
        {
            Assert.That(_vm.GetFirstMediaItemIndex(), Is.EqualTo(_vm.MediaItems.IndexOf(TestData.MediaItems.First())));
        }

        [Test]
        public void GetLastMediaItemIndex_WhenCalled_ReturnsIndexOfLastMediaItemInMediaList()
        {
            Assert.That(_vm.GetLastMediaItemIndex(), Is.EqualTo(_vm.MediaItems.IndexOf(TestData.MediaItems.Last())));
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

        [Test]
        public void IsEndOfCurrentlyPlayingMedia_IsNotTheEnd_ReturnsFalse()
        {
            _vm.SelectMediaItem(_vm.GetFirstMediaItemIndex());

            _vm.SelectedMediaItem.Duration = TimeSpan.FromSeconds(300);
            _vm.SelectedMediaItem.ElapsedTime = TimeSpan.FromSeconds(100);

            Assert.That(_vm.IsEndOfCurrentlyPlayingMedia(), Is.EqualTo(false));
        }

        [Test]
        public void IsEndOfCurrentlyPlayingMedia_IsTheEnd_ReturnsTrue()
        {
            _vm.SelectMediaItem(_vm.GetFirstMediaItemIndex());

            _vm.SelectedMediaItem.Duration = TimeSpan.FromSeconds(300);
            _vm.SelectedMediaItem.ElapsedTime = TimeSpan.FromSeconds(300);

            Assert.That(_vm.IsEndOfCurrentlyPlayingMedia(), Is.EqualTo(true));
        }

        public static class TestData
        {
            public static IEnumerable<string> FilePaths = new List<string>()
            {
                "Fakedir/Track 1",
                "Fakedir/Track 2",
                "Fakedir/Track 3"
            };

            public static IEnumerable<MediaItem> MediaItems = new List<MediaItem>()
            {
                new AudioItem
                {
                    Id = 1,
                    Lyrics = "These are lyrics",
                    Album = "Test Album",
                    MediaTitle = "Track 1"
                },
                new AudioItem
                {
                    Id = 2,
                    Lyrics = "These are lyrics, too",
                    Album = "Test Album",
                    MediaTitle = "Track 2"
                },
                new AudioItem
                {
                    Id = 3,
                    Lyrics = "I am singing, here are some lyrics woo",
                    Album = "Test Album",
                    MediaTitle = "Track 3"
                }
            };
        }
    }
}
