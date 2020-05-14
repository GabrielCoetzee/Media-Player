using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System.Windows.Forms;
using MediaPlayer.BusinessEntities.Collections;
using MediaPlayer.BusinessEntities.Objects.Base;
using ListBox = System.Windows.Controls.ListBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using MediaPlayer.BusinessLogic;
using MediaPlayer.ApplicationSettings.Config;
using Microsoft.Extensions.Options;
using MediaPlayer.ApplicationSettings;
using Generic.Commands;
using Generic.Mediator;

namespace MediaPlayer.ViewModel
{
    public class ViewModelMediaPlayer : INotifyPropertyChanged
    {
        #region Injected Properties

        public ISettingsProvider SettingsProvider { get; set; }

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

        public ViewModelMediaPlayer(ISettingsProvider settingsProvider, MetadataReaderProviderResolver metadataReaderProviderResolver)
        {
            this.SettingsProvider = settingsProvider;

            this.MetadataReaderProviderResolver = metadataReaderProviderResolver;

            InitializeCommands();
            InitializeEventTriggerCommands();
            InitializeModel();
        }

        #endregion

        #region Initialization

        private void InitializeModel()
        {
            ModelMediaPlayer = new ModelMediaPlayer()
            {
                MediaItems = new MediaItemObservableCollection(),
                MediaState = MediaState.Pause,
                MediaVolume = VolumeLevel.Full
            };
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
            ShuffleMediaListCommand = new RelayCommand(ShuffleMediaListCommand_Execute, ShuffleMediaListCommand_CanExecute);
            ClearMediaListCommand = new RelayCommand(ClearMediaListCommand_Execute, ClearMediaListCommand_CanExecute);
            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindowCommand_Execute, OpenSettingsWindowCommand_CanExecute);
        }

        private void InitializeEventTriggerCommands()
        {
            MediaOpenedCommand = new RelayCommandWithParameter(MediaOpenedCommand_Execute, MediaOpenedCommand_CanExecute);
            SeekbarValueChangedCommand = new RelayCommand(SeekbarValueChangedCommand_Execute, SeekbarValueChangedCommand_CanExecute);
            SeekbarThumbStartedDraggingCommand = new RelayCommand(SeekbarThumbStartedCommand_Execute, SeekbarThumbStartedDraggingCommand_CanExecute);
            SeekbarThumbCompletedDraggingCommand = new RelayCommand(SeekbarThumbCompletedDraggingCommand_Execute, SeekbarThumbCompletedDraggingCommand_CanExecute);
        }

        #endregion

        #region Command Methods

        public bool OpenSettingsWindowCommand_CanExecute()
        {
            return true;
        }

        public void OpenSettingsWindowCommand_Execute()
        {
            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.OpenApplicationSettings);
        }

        public bool ClearMediaListCommand_CanExecute()
        {
            return !this.ModelMediaPlayer.IsMediaListEmpty();
        }

        public void ClearMediaListCommand_Execute()
        {
            _mediaPositionTracker.Stop();

            this.InitializeModel();
        }

        public bool ShuffleMediaListCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaItems.Count > 2 && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void ShuffleMediaListCommand_Execute()
        {
            if (ModelMediaPlayer.IsMediaItemsShuffled)
                this.ModelMediaPlayer.OrderMediaList();
            else
                this.ModelMediaPlayer.ShuffleMediaList();

            ModelMediaPlayer.IsMediaItemsShuffled = !ModelMediaPlayer.IsMediaItemsShuffled;
        }

        public bool RepeatMediaListCommand_CanExecute()
        {
            return !this.ModelMediaPlayer.IsMediaListEmpty() && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void RepeatMediaListCommand_Execute()
        {
            ModelMediaPlayer.IsRepeatEnabled = !ModelMediaPlayer.IsRepeatEnabled;
        }

        public bool NextTrackCommand_CanExecute()
        {
            return !this.ModelMediaPlayer.IsMediaListEmpty() && this.ModelMediaPlayer.IsNextMediaItemAvailable() || ModelMediaPlayer.IsRepeatEnabled;
        }

        public void NextTrackCommand_Execute()
        {
            this.ModelMediaPlayer.PlayNextMediaItem();
        }   

        public bool StopCommand_CanExecute()
        {
            return !this.ModelMediaPlayer.IsMediaListEmpty() && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void StopCommand_Execute()
        {
            this.ModelMediaPlayer.SelectMediaItem(this.ModelMediaPlayer.GetFirstMediaItemIndex());

            this.ModelMediaPlayer.StopMedia();
        }

        public bool PreviousTrackCommand_CanExecute()
        {
            return !this.ModelMediaPlayer.IsMediaListEmpty() && this.ModelMediaPlayer.IsPreviousMediaItemAvailable() || ModelMediaPlayer.IsRepeatEnabled; 
        }

        public void PreviousTrackCommand_Execute()
        {
            this.ModelMediaPlayer.PlayPreviousMediaItem();
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
                this.ModelMediaPlayer.PlayMedia();
            else   
                this.ModelMediaPlayer.PauseMedia();
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
                DefaultExt = this.SettingsProvider.SupportedFileFormats.First(),
                Filter = CreateDialogFilter(),
                Multiselect = true
            };

            var result = chooseFiles.ShowDialog();

            if (result != DialogResult.OK)
                return;

            var metadataReader = MetadataReaderProviderResolver.Resolve(Common.Enumerations.MetadataReaders.Taglib);

            var mediaItems = chooseFiles.FileNames.Select(file => metadataReader.GetFileMetadata(file)).ToList();

            AddToMediaList(mediaItems);
        }

        #endregion

        #region Event Trigger Command Methods

        public bool MediaOpenedCommand_CanExecute()
        {
            return !this.ModelMediaPlayer.IsMediaListEmpty() && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void MediaOpenedCommand_Execute(object uiElement)
        {
            if (!(uiElement is MediaElement UI_MediaElement))
                return;

            PollMediaPosition(UI_MediaElement);
        }

        public bool SeekbarThumbCompletedDraggingCommand_CanExecute()
        {
            return ModelMediaPlayer.IsUserDraggingSeekbarThumb;
        }

        public void SeekbarThumbCompletedDraggingCommand_Execute()
        {
            ModelMediaPlayer.IsUserDraggingSeekbarThumb = false;

            this.ModelMediaPlayer.MediaPosition = this.ModelMediaPlayer.SelectedMediaItem.ElapsedTime;
        }

        public bool SeekbarThumbStartedDraggingCommand_CanExecute()
        {
            return ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void SeekbarThumbStartedCommand_Execute()
        {
            ModelMediaPlayer.IsUserDraggingSeekbarThumb = true;
        }

        public bool SeekbarValueChangedCommand_CanExecute()
        {
            return ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void SeekbarValueChangedCommand_Execute()
        {
            if (ModelMediaPlayer.IsUserDraggingSeekbarThumb)
            {
                this.ModelMediaPlayer.MediaPosition = this.ModelMediaPlayer.SelectedMediaItem.ElapsedTime;
            }
        }

        #endregion

        #region Public Methods

        public void AddToMediaList(IEnumerable<MediaItem> mediaItems)
        {
            this.ModelMediaPlayer.MediaItems.AddRange(mediaItems);

            if (ModelMediaPlayer.SelectedMediaItem != null || this.ModelMediaPlayer.IsMediaListEmpty())
                return;

            this.ModelMediaPlayer.SelectMediaItem(this.ModelMediaPlayer.GetFirstMediaItemIndex());
            this.ModelMediaPlayer.PlayMedia();

            this.RefreshUIBindings();
        }

        public void SetIsLoadingMediaItems(bool isLoadingMediaItems)
        {
            ModelMediaPlayer.IsLoadingMediaItems = isLoadingMediaItems;
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
            return this.SettingsProvider.SupportedFileFormats.Aggregate(string.Empty, (current, format) => current + $"*{format}{(this.SettingsProvider.SupportedFileFormats.Last() != format ? seperator : string.Empty)}");
        }

        private void PollMediaPosition(MediaElement mediaElement)
        {
            if (mediaElement == null)
                return;

            this.ModelMediaPlayer.SetAccurateCurrentMediaDuration(mediaElement.NaturalDuration.TimeSpan);

            _mediaPositionTracker.Tick += (sender, args) => UpdateMediaPosition(mediaElement);

            _mediaPositionTracker.Start();
        }

        private void UpdateMediaPosition(MediaElement mediaElement)
        {
            if (!ModelMediaPlayer.IsUserDraggingSeekbarThumb)
            {
                this.ModelMediaPlayer.MediaPosition = mediaElement.Position;
            }

            if (!this.ModelMediaPlayer.IsEndOfCurrentMedia(this.ModelMediaPlayer.SelectedMediaItem.ElapsedTime))
                return;

            if (NextTrackCommand_CanExecute())
            {
                NextTrackCommand_Execute();
            }

            this.RefreshUIBindings();
        }

        #endregion
    }
}
