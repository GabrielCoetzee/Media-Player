using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MediaPlayer.ApplicationSettings.Interfaces;
using MediaPlayer.BusinessEntities.Collections.Derived;
using MediaPlayer.BusinessEntities.Enumerations;
using MediaPlayer.BusinessEntities.Objects.Abstract;
using MediaPlayer.Generic.Commands;
using MediaPlayer.Generic.Window_Service.Interface_Implementations;
using MediaPlayer.Metadata_Readers;
using MediaPlayer.MVVM.Models;
using MediaPlayer.MVVM.Views;
using Ninject;
using ListBox = System.Windows.Controls.ListBox;

namespace MediaPlayer.MVVM.ViewModels
{
    public class ViewModelMediaPlayer : INotifyPropertyChanged
    {
        #region Injected Properties

        [Inject]
        public IExposeApplicationSettings ApplicationSettings { get; set; }

        [Inject]
        public MetadataReaderProviderResolver MetadataReaderProviderResolver { get; set; }

        #endregion

        #region Interface Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        #region Fields

        private readonly Random _randomIdGenerator = new Random();
        private readonly DispatcherTimer _mediaPositionTracker = new DispatcherTimer();

        private ModelMediaPlayer _modelMediaPlayer;

        private ICommand _addMediaCommand;
        private ICommand _playPauseCommand;
        private ICommand _muteCommand;
        private ICommand _previousTrackCommand;
        private ICommand _stopCommand;
        private ICommand _nextTrackCommand;
        private ICommand _mediaOpenedCommand;
        private ICommand _seekbarValueChangedCommand;
        private ICommand _seekbarThumbStartedDraggingCommand;
        private ICommand _seekbarThumbCompletedDraggingCommand;
        private ICommand _repeatMediaListCommand;
        private ICommand _shuffleMediaListCommand;
        private ICommand _clearMediaListCommand;
        private ICommand _openSettingsWindowCommand;

        #endregion

        #region Properties

        public ModelMediaPlayer ModelMediaPlayer
        {
            get => _modelMediaPlayer;
            set
            {
                _modelMediaPlayer = value;
                OnPropertyChanged(nameof(ModelMediaPlayer));
            }
        }

        public ICommand AddMediaCommand
        {
            get => _addMediaCommand;
            set
            {
                _addMediaCommand = value; 
                OnPropertyChanged(nameof(AddMediaCommand));
            }
        }

        public ICommand PlayPauseCommand
        {
            get => _playPauseCommand;
            set
            {
                _playPauseCommand = value;
                OnPropertyChanged(nameof(PlayPauseCommand));
            }
        }

        public ICommand MuteCommand
        {
            get => _muteCommand;
            set
            {
                _muteCommand = value;
                OnPropertyChanged(nameof(MuteCommand));
            }
        }

        public ICommand PreviousTrackCommand
        {
            get => _previousTrackCommand;
            set
            {
                _previousTrackCommand = value;
                OnPropertyChanged(nameof(PreviousTrackCommand));
            }
        }

        public ICommand StopCommand
        {
            get => _stopCommand;
            set
            {
                _stopCommand = value;
                OnPropertyChanged(nameof(StopCommand));
            }
        }

        public ICommand NextTrackCommand
        {
            get => _nextTrackCommand;
            set
            {
                _nextTrackCommand = value;
                OnPropertyChanged(nameof(NextTrackCommand));
            }
        }

        public ICommand MediaOpenedCommand
        {
            get => _mediaOpenedCommand;
            set
            {
                _mediaOpenedCommand = value;
                OnPropertyChanged(nameof(MediaOpenedCommand));
            }
        }

        public ICommand SeekbarValueChangedCommand
        {
            get => _seekbarValueChangedCommand;
            set
            {
                _seekbarValueChangedCommand = value;
                OnPropertyChanged(nameof(SeekbarValueChangedCommand));
            }
        }

        public ICommand SeekbarThumbStartedDraggingCommand
        {
            get => _seekbarThumbStartedDraggingCommand;
            set
            {
                _seekbarThumbStartedDraggingCommand = value;
                OnPropertyChanged(nameof(SeekbarThumbStartedDraggingCommand));
            }
        }

        public ICommand SeekbarThumbCompletedDraggingCommand
        {
            get => _seekbarThumbCompletedDraggingCommand;
            set
            {
                _seekbarThumbCompletedDraggingCommand = value;
                OnPropertyChanged(nameof(SeekbarThumbCompletedDraggingCommand));
            }
        }

        public ICommand RepeatMediaListCommand
        {
            get => _repeatMediaListCommand;
            set
            {
                _repeatMediaListCommand = value;
                OnPropertyChanged(nameof(RepeatMediaListCommand));
            }
        }

        public ICommand ShuffleMediaListCommand
        {
            get => _shuffleMediaListCommand;
            set
            {
                _shuffleMediaListCommand = value;
                OnPropertyChanged(nameof(ShuffleMediaListCommand));
            }
        }

        public ICommand ClearMediaListCommand
        {
            get => _clearMediaListCommand;
            set
            {
                _clearMediaListCommand = value;
                OnPropertyChanged(nameof(ClearMediaListCommand));
            }
        }

        public ICommand OpenSettingsWindowCommand
        {
            get => _openSettingsWindowCommand;
            set
            {
                _openSettingsWindowCommand = value;
                OnPropertyChanged(nameof(OpenSettingsWindowCommand));
            }
        }

        #endregion

        #region Constructor

        public ViewModelMediaPlayer()
        {
            InitializeCommands();
            InitializeEventTriggerCommands();
            InitializeModelInstance();
        }

        #endregion

        #region Initialization

        private void InitializeModelInstance()
        {
            ModelMediaPlayer = new ModelMediaPlayer() { MediaList = new MediaItemObservableCollection(), MediaState = MediaState.Pause, MediaVolume = VolumeLevel.Full};
        }

        private void InitializeCommands()
        {
            AddMediaCommand = new RelayCommand(AddFilesCommand_Execute, AddFilesCommand_CanExecute);
            PlayPauseCommand = new RelayCommand(PlayPauseCommand_Execute, PlayPauseCommand_CanExecute);
            MuteCommand = new RelayCommand(MuteCommand_Execute, MuteCommand_CanExecute);
            PreviousTrackCommand = new RelayCommand(PreviousTrackCommand_Execute, PreviousTrackCommand_CanExecute);
            StopCommand = new RelayCommand(StopCommand_Execute, StopCommand_CanExecute);
            NextTrackCommand = new RelayCommand(NextTrackCommand_Execute, NextTrackCommand_CanExecute);
            RepeatMediaListCommand = new RelayCommand(RepeatMediaListCommand_Execute, RepeatMediaListCommand_CanExecute);
            ShuffleMediaListCommand = new RelayCommandWithParameter(ShuffleMediaListCommand_Execute, ShuffleMediaListCommand_CanExecute);
            ClearMediaListCommand = new RelayCommand(ClearMediaListCommand_Execute, ClearMediaListCommand_CanExecute);
            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindowCommand_Execute, OpenSettingsWindowCommand_CanExecute);
        }

        private void InitializeEventTriggerCommands()
        {
            MediaOpenedCommand = new RelayCommandWithParameter(MediaOpenedCommand_Execute, MediaOpenedCommand_CanExecute);
            SeekbarValueChangedCommand = new RelayCommandWithParameter(SeekbarValueChangedCommand_Execute, SeekbarValueChangedCommand_CanExecute);
            SeekbarThumbStartedDraggingCommand = new RelayCommand(SeekbarThumbStartedCommand_Execute, SeekbarThumbStartedDraggingCommand_CanExecute);
            SeekbarThumbCompletedDraggingCommand = new RelayCommandWithParameter(SeekbarThumbCompletedDraggingCommand_Execute, SeekbarThumbCompletedDraggingCommand_CanExecute);
        }

        #endregion

        #region Command Methods

        public bool OpenSettingsWindowCommand_CanExecute()
        {
            return true;
        }

        public void OpenSettingsWindowCommand_Execute()
        {
            var windowService = new WindowService<ViewApplicationSettings>();
            windowService.ShowWindowModal(ApplicationSettings);
        }

        public bool ClearMediaListCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 0;
        }

        public void ClearMediaListCommand_Execute()
        {
            _mediaPositionTracker.Stop();
            InitializeModelInstance();
        }

        public bool ShuffleMediaListCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 2 && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void ShuffleMediaListCommand_Execute(object uiElement)
        {
            if (!(uiElement is ListBox UI_MediaList))
                return;

            ModelMediaPlayer.IsShuffled = !ModelMediaPlayer.IsShuffled;

            if (ModelMediaPlayer.IsShuffled)
                ShuffleMediaList();
            else
                OrderMediaList();

            UI_MediaList.ScrollIntoView(UI_MediaList.SelectedItem);
        }

        public bool RepeatMediaListCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void RepeatMediaListCommand_Execute()
        {
            ModelMediaPlayer.IsRepeatMediaListEnabled = !ModelMediaPlayer.IsRepeatMediaListEnabled;
        }

        public bool NextTrackCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && NextMediaItemIsAvailable() || ModelMediaPlayer.IsRepeatMediaListEnabled;
        }

        public void NextTrackCommand_Execute()
        {
            PlayNextMediaItem();
        }   

        public bool StopCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void StopCommand_Execute()
        {
            SelectMediaItem(GetFirstMediaItemIndex());
            StopMedia();
        }

        public bool PreviousTrackCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && PreviousMediaItemIsAvailable() || ModelMediaPlayer.IsRepeatMediaListEnabled; 
        }

        public void PreviousTrackCommand_Execute()
        {
            PlayPreviousMediaItem();
        }

        public bool MuteCommand_CanExecute()
        {
            return ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void MuteCommand_Execute()
        {
            ModelMediaPlayer.MediaVolume = ModelMediaPlayer.MediaVolume == VolumeLevel.Full ? VolumeLevel.Mute : VolumeLevel.Full;
        }

        public bool PlayPauseCommand_CanExecute()
        {
            return ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void PlayPauseCommand_Execute()
        {
            if (ModelMediaPlayer.MediaState != MediaState.Play)
                PlayMedia();
            else   
                PauseMedia();
        }

        public bool AddFilesCommand_CanExecute()
        {
            return true;
        }

        public void AddFilesCommand_Execute()
        {
            var chooseFiles = new OpenFileDialog
            {
                Title = "Choose Files",
                DefaultExt = ApplicationSettings.SupportedFormats.First(),
                Filter = CreateDialogFilter(),
                Multiselect = true
            };

            var result = chooseFiles.ShowDialog();

            if (result != DialogResult.OK)
                return;

            var metadataReader = MetadataReaderProviderResolver.Resolve(MetadataReaders.Taglib);

            var mediaItems = chooseFiles.FileNames.Select(file => metadataReader.GetFileMetadata(file)).ToList();
            AddToMediaList(mediaItems);
        }

        #endregion

        #region Event Trigger Command Methods

        public bool MediaOpenedCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void MediaOpenedCommand_Execute(object uiElement)
        {
            if (!(uiElement is MediaElement UI_MediaElement))
                return;

            PollMediaPosition(UI_MediaElement);
        }

        public bool SeekbarThumbCompletedDraggingCommand_CanExecute()
        {
            return ModelMediaPlayer.IsDraggingSeekbarThumb;
        }

        public void SeekbarThumbCompletedDraggingCommand_Execute(object uiElement)
        {
            if (!(uiElement is MediaElement UI_MediaElement))
                return;

            ModelMediaPlayer.IsDraggingSeekbarThumb = false;
            UI_MediaElement.Position = TimeSpan.FromSeconds(ModelMediaPlayer.ElapsedTime);
        }

        public bool SeekbarThumbStartedDraggingCommand_CanExecute()
        {
            return ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void SeekbarThumbStartedCommand_Execute()
        {
            ModelMediaPlayer.IsDraggingSeekbarThumb = true;
        }

        public bool SeekbarValueChangedCommand_CanExecute()
        {
            return ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void SeekbarValueChangedCommand_Execute(object uiElement)
        {
            if (!(uiElement is Slider UI_Seekbar))
                return;

            if (ModelMediaPlayer.IsDraggingSeekbarThumb)
                ModelMediaPlayer.MediaPosition = TimeSpan.FromSeconds(UI_Seekbar.Value);
        }

        #endregion

        #region Public Methods

        public void AddToMediaList(IEnumerable<string> files)
        {
            var metadataReader = MetadataReaderProviderResolver.Resolve(MetadataReaders.Taglib);

            foreach (var file in files)
                ModelMediaPlayer.MediaList.Add(metadataReader.GetFileMetadata(file));

            if (ModelMediaPlayer.SelectedMediaItem != null || ModelMediaPlayer.MediaList.Count <= 0)
                return;

            SelectMediaItem(GetFirstMediaItemIndex());
            PlayMedia();
        }

        public void AddToMediaList(IEnumerable<MediaItem> mediaItems)
        {
            ModelMediaPlayer.MediaList.AddRange(mediaItems);

            if (ModelMediaPlayer.SelectedMediaItem != null || ModelMediaPlayer.MediaList.Count <= 0)
                return;

            SelectMediaItem(GetFirstMediaItemIndex());
            PlayMedia();

            RefreshUIBindings();
        }

        #endregion

        #region Private Methods

        private void RefreshUIBindings()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private string CreateDialogFilter()
        {
            return string.Join("|", $"Supported Formats ({AppendSupportedFormats(",")})", AppendSupportedFormats(";"));
        }

        private string AppendSupportedFormats(string seperator)
        {
            return ApplicationSettings.SupportedFormats.Aggregate(string.Empty, (current, format) => current + $"*{format}{(ApplicationSettings.SupportedFormats.Last() != format ? seperator : string.Empty)}");
        }

        private void PlayMedia()
        {
            ModelMediaPlayer.MediaState = MediaState.Play;
        }

        private void PauseMedia()
        {
            ModelMediaPlayer.MediaState = MediaState.Pause;
        }

        private void StopMedia()
        {
            ModelMediaPlayer.MediaState = MediaState.Stop;
        }

        private void SelectMediaItem(int index)
        {
            ModelMediaPlayer.SelectedMediaItem = null;
            ModelMediaPlayer.SelectedMediaItem = ModelMediaPlayer.MediaList[index];
        }

        private bool PreviousMediaItemIsAvailable()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && IsPreviousMediaItemAvailable();
        }

        private bool NextMediaItemIsAvailable()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && IsNextMediaItemAvailable();
        }

        private void PlayPreviousMediaItem()
        {
            if (ModelMediaPlayer.IsRepeatMediaListEnabled && IsFirstMediaItemSelected())
                SelectMediaItem(GetLastMediaItemIndex());
            else
                SelectMediaItem(GetPreviousMediaItemIndex());

            PlayMedia();
        }

        private void PlayNextMediaItem()
        {
            if (ModelMediaPlayer.IsRepeatMediaListEnabled && IsLastMediaItemSelected())
                SelectMediaItem(GetFirstMediaItemIndex());
            else
                SelectMediaItem(GetNextMediaItemIndex());

            PlayMedia();
        }

        private void OrderMediaList()
        {
            ModelMediaPlayer.MediaList = new MediaItemObservableCollection(ModelMediaPlayer.MediaList.OrderBy(x => x.Id));
        }

        private void ShuffleMediaList()
        {
            ModelMediaPlayer.MediaList = new MediaItemObservableCollection(ModelMediaPlayer.MediaList
                .OrderBy(x => x != ModelMediaPlayer.SelectedMediaItem)
                .ThenBy(x => _randomIdGenerator.Next()));
        }

        private void PollMediaPosition(MediaElement mediaElement)
        {
            if (mediaElement != null)
            {
                _mediaPositionTracker.Tick += (sender, args) => UpdateMediaPosition(mediaElement);
            }

            _mediaPositionTracker.Start();
        }

        private void UpdateMediaPosition(MediaElement mediaElement)
        {
            if (!ModelMediaPlayer.IsDraggingSeekbarThumb)
            {
                ModelMediaPlayer.MediaPosition = mediaElement.Position;
            }

            if (mediaElement.NaturalDuration.HasTimeSpan && !CurrentMediaDurationIsAccurate(mediaElement))
            {
                SetAccurateCurrentMediaDuration(mediaElement.NaturalDuration.TimeSpan);
            }

            if (IsEndOfCurrentMedia(mediaElement))
            {
                if (NextTrackCommand_CanExecute())
                {
                    NextTrackCommand_Execute();
                }

                RefreshUIBindings();
            }
        }

        private void SetAccurateCurrentMediaDuration(TimeSpan mediaDuration)
        {
            ModelMediaPlayer.SelectedMediaItem.MediaDuration = mediaDuration;
        }

        private bool IsEndOfCurrentMedia(MediaElement mediaElement)
        {
            return mediaElement.Position.TotalSeconds == ModelMediaPlayer.SelectedMediaItem?.MediaDuration.TotalSeconds;
        }

        private bool CurrentMediaDurationIsAccurate(MediaElement mediaElement)
        {
            return ModelMediaPlayer.SelectedMediaItem?.MediaDuration == mediaElement.NaturalDuration.TimeSpan;
        }

        private bool IsFirstMediaItemSelected()
        {
            return ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) == ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.First());
        }

        private bool IsLastMediaItemSelected()
        {
            return ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) == ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.Last());
        }

        private int GetFirstMediaItemIndex()
        {
            return ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.First());
        }

        private int GetLastMediaItemIndex()
        {
            return ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.Last());
        }

        private int GetPreviousMediaItemIndex()
        {
            return ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) - 1;
        }

        private int GetNextMediaItemIndex()
        {
            return ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) + 1;
        }
        private bool IsPreviousMediaItemAvailable()
        {
            return ModelMediaPlayer.MediaList.Any(x => ModelMediaPlayer.MediaList.IndexOf(x) == ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) - 1);
        }
        private bool IsNextMediaItemAvailable()
        {
            return ModelMediaPlayer.MediaList.Any(x => ModelMediaPlayer.MediaList.IndexOf(x) == ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) + 1);
        }

        #endregion
    }
}
