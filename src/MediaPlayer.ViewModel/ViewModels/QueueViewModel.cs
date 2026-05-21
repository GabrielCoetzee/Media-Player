using Generic.Mediator;
using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.BusinessEntities.Concrete;
using MediaPlayer.Model.Collections;
using MediaPlayer.ViewModel.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel.ViewModels
{
    [Export]
    public class QueueViewModel : NotifyPropertyChanged
    {
        private MediaItem _selectedMediaItem;
        public MediaItem SelectedMediaItem
        {
            get => _selectedMediaItem;
            set
            {
                _selectedMediaItem = value;
                OnPropertyChanged(nameof(SelectedMediaItem));

                Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
            }
        }

        public MediaItemObservableCollection MediaItems { get; } = [];

        public bool IsMediaListPopulated => MediaItems.Count > 0;

        [Import]
        public IMetadataServices MetadataServices { get; set; }

        [Import]
        public IMediaLoader MediaLoader { get; set; }

        [Import]
        public BusyViewModel BusyViewModel { get; set; }

        [Import]
        public SettingsViewModel SettingsViewModel { get; set; }

        [Import(CommandNames.ClearList)]
        public ICommand ClearMediaListCommand { get; set; }

        [Import(CommandNames.AddMedia)]
        public ICommand AddMediaCommand { get; set; }

        [Import(CommandNames.RemoveMediaItem)]
        public ICommand RemoveMediaItemCommand { get; set; }

        public QueueViewModel()
        {
            MediaItems.CollectionChanged += MediaItems_CollectionChanged;
        }

        private void MediaItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsMediaListPopulated));
        }

        public async Task AddMediaAsync(IEnumerable<string> paths)
        {
            if (paths == null || !paths.Any())
                return;

            BusyViewModel.MediaListLoading();

            var newlyAddedItems = new List<MediaItem>();

            try
            {
                await foreach (var batch in MediaLoader.LoadInBatchesAsync(paths))
                {
                    newlyAddedItems.AddRange(batch);
                    AddMediaItemsToListView(batch);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }

            BusyViewModel.MediaListPopulated();

            await UpdateMetadataAsync(newlyAddedItems.OfType<AudioItem>());
        }

        public void RemoveMediaItem(MediaItem item)
        {
            if (item == null || !MediaItems.Contains(item))
                return;

            var isCurrentlyPlaying = ReferenceEquals(item, SelectedMediaItem);

            if (!isCurrentlyPlaying)
            {
                MediaItems.Remove(item);
                return;
            }

            if (MediaItems.Count == 1)
            {
                SelectedMediaItem = null;
                MediaItems.Remove(item);
                return;
            }

            var nextIndex = IsLastMediaItemSelected()
                ? GetPreviousMediaItemIndex()
                : GetNextMediaItemIndex();

            SelectMediaItem(nextIndex);
            MediaItems.Remove(item);
        }

        public async Task SaveDirtyMetadataAsync()
        {
            if (!SettingsViewModel.SaveMetadataToFile)
                return;

            BusyViewModel.SavingChanges();

            await MetadataServices.MetadataWriter.WriteChangesToFilesInParallel(MediaItems.Where(x => x.IsDirty));
        }

        public async Task ClearMediaListAsync()
        {
            MediaLoader.Cancel();
            MetadataServices.MetadataUpdater.Cancel();

            SelectedMediaItem = null;

            await SaveDirtyMetadataAsync();

            MediaItems.Clear();

            BusyViewModel.InitialStartupState();
        }

        private void AddMediaItemsToListView(IReadOnlyList<MediaItem> mediaItems)
        {
            MediaItems.AddRange(mediaItems);

            if (SelectedMediaItem != null)
                return;

            SelectMediaItem(GetFirstMediaItemIndex());

            CommandManager.InvalidateRequerySuggested();
        }

        private async Task UpdateMetadataAsync(IEnumerable<AudioItem> audioItems)
        {
            if (!SettingsViewModel.UpdateMetadata || !audioItems.Any())
                return;

            BusyViewModel.UpdatingMetadata();

            try
            {
                await MetadataServices.MetadataUpdater.UpdateMetadataAsync(audioItems);

                BusyViewModel.MediaListPopulated();

                MetadataServices.MetadataCorrector.FixMetadata(audioItems);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void SelectMediaItem(int index) => SelectedMediaItem = MediaItems[index];

        public bool IsPreviousMediaItemAvailable() => IsMediaListPopulated && GetPreviousMediaItemIndex() >= GetFirstMediaItemIndex();

        public bool IsNextMediaItemAvailable() => IsMediaListPopulated && GetNextMediaItemIndex() <= GetLastMediaItemIndex();

        public int GetPreviousMediaItemIndex() => MediaItems.IndexOf(SelectedMediaItem) - 1;

        public int GetNextMediaItemIndex() => MediaItems.IndexOf(SelectedMediaItem) + 1;

        public int GetFirstMediaItemIndex() => MediaItems.IndexOf(MediaItems.First());

        public int GetLastMediaItemIndex() => MediaItems.IndexOf(MediaItems.Last());

        public bool IsFirstMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == GetFirstMediaItemIndex();

        public bool IsLastMediaItemSelected() => MediaItems.IndexOf(SelectedMediaItem) == GetLastMediaItemIndex();
    }
}
