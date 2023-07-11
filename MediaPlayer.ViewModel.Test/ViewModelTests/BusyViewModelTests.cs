using MediaPlayer.ViewModel.ViewModels;
using NUnit.Framework;

namespace MediaPlayer.ViewModel.Test.ViewModelTests
{
    [TestFixture]
    public class BusyViewModelTests
    {
        BusyViewModel _vm;

        [SetUp]
        public void SetUp()
        {
            _vm = new BusyViewModel();
        }

        [Test]
        public void InitialStartupState_WhenCalled_HasNoMediaListTitleAndNoLoadingIndicator()
        {
            _vm.InitialStartupState();

            Assert.Multiple(() =>
            {
                Assert.That(_vm.IsLoading, Is.EqualTo(false));
                Assert.That(_vm.MediaListTitle, Is.EqualTo(string.Empty));
            });
        }

        [Test]
        public void MediaListLoading_WhenCalled_HasLoadingIndicatorAndLoadingText()
        {
            _vm.MediaListLoading();

            Assert.Multiple(() =>
            {
                Assert.That(_vm.IsLoading, Is.EqualTo(true));
                Assert.That(_vm.MediaListTitle, Is.EqualTo("Media List Loading..."));
            });
        }

        [Test]
        public void MediaListPopulated_WhenCalled_HasDefaultTextAndNoLoadingIndicator()
        {
            _vm.MediaListPopulated();

            Assert.Multiple(() =>
            {
                Assert.That(_vm.IsLoading, Is.EqualTo(false));
                Assert.That(_vm.MediaListTitle, Is.EqualTo("Media List"));
            });
        }

        [Test]
        public void UpdatingMetadata_WhenCalled_HasLoadingIndicatorAndUpdateText()
        {
            _vm.UpdatingMetadata();

            Assert.Multiple(() =>
            {
                Assert.That(_vm.IsLoading, Is.EqualTo(true));
                Assert.That(_vm.MediaListTitle, Is.EqualTo("Updating Metadata..."));
            });
        }

        [Test]
        public void SavingChanges_WhenCalled_HasLoadingIndicatorAndSaveChangesText()
        {
            _vm.SavingChanges();

            Assert.Multiple(() =>
            {
                Assert.That(_vm.IsLoading, Is.EqualTo(true));
                Assert.That(_vm.MediaListTitle, Is.EqualTo("Saving Changes..."));
            });
        }
    }
}
