using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MediaPlayer.Helpers.Custom_Command_Classes;
using MediaPlayer.Interfaces;
using MediaPlayer.MetadataReaders.Factory;
using MediaPlayer.MetadataReaders.Interfaces;
using MediaPlayer.MetadataReaders.Types;
using MediaPlayer.MVVM.Models;
using MediaPlayer.MVVM.Models.Base_Types;
using MediaPlayer.MVVM.Models.Objects;
using MediaPlayer.Objects;
using Microsoft.Win32;

namespace MediaPlayer.MVVM.ViewModels
{
    public class ViewModelMediaPlayer : INotifyPropertyChanged
    {
        #region Interface Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        #region Fields

        readonly Random RandomIdGenerator = new Random();
        readonly DispatcherTimer MediaPositionTracker = new DispatcherTimer();

        private ModelMediaPlayer _modelMediaPlayer;  
        private IReadMp3Metadata _readMp3Metadata;

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

        public IExposeApplicationSettings Settings => ApplicationSettings.Instance;

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

        #endregion

        #region Constructor

        public ViewModelMediaPlayer()
        {
            InitializeCommands();
            InitializeEventTriggerCommands();
            InitializeModelInstance();
            GetMp3MetadataReader();
        }

        #endregion

        #region Initialization

        private void InitializeModelInstance()
        {
            ModelMediaPlayer = new ModelMediaPlayer() { MediaList = new ObservableCollection<MediaItem>(), MediaState = MediaState.Pause, MediaVolume = CustomTypes.VolumeLevel.FullVolume};
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
        }

        private void InitializeEventTriggerCommands()
        {
            MediaOpenedCommand = new RelayCommandWithParameter(MediaOpenedCommand_Execute, MediaOpenedCommand_CanExecute);
            SeekbarValueChangedCommand = new RelayCommandWithParameter(SeekbarValueChangedCommand_Execute, SeekbarValueChangedCommand_CanExecute);
            SeekbarThumbStartedDraggingCommand = new RelayCommand(SeekbarThumbStartedCommand_Execute, SeekbarThumbStartedDraggingCommand_CanExecute);
            SeekbarThumbCompletedDraggingCommand = new RelayCommandWithParameter(SeekbarThumbCompletedDraggingCommand_Execute, SeekbarThumbCompletedDraggingCommand_CanExecute);
        }

        private void GetMp3MetadataReader()
        {
            _readMp3Metadata = Mp3MetadataReaderFactory.Instance.GetMp3MetadataReader(Mp3MetadataReaderTypes.Mp3MetadataReaders.Taglib);
        }

        #endregion

        #region Command Methods

        public bool ClearMediaListCommand_CanExecute()
        {
            return ModelMediaPlayer.SelectedMediaItem != null && ModelMediaPlayer.MediaList.Count > 0;
        }

        public void ClearMediaListCommand_Execute()
        {
            MediaPositionTracker.Stop();
            InitializeModelInstance();
        }

        public bool ShuffleMediaListCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 2 && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void ShuffleMediaListCommand_Execute(object uiElement)
        {
            if (uiElement is ListBox UI_MediaList)
            {
                ModelMediaPlayer.IsShuffled = !ModelMediaPlayer.IsShuffled;

                if (ModelMediaPlayer.IsShuffled)
                    ShuffleMediaList();
                else
                    OrderMediaList();

                UI_MediaList.ScrollIntoView(UI_MediaList.SelectedItem);
            }
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
            SelectMediaItem(ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.First()));
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
            ModelMediaPlayer.MediaVolume = ModelMediaPlayer.MediaVolume == CustomTypes.VolumeLevel.FullVolume ? CustomTypes.VolumeLevel.Mute : CustomTypes.VolumeLevel.FullVolume;
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
                DefaultExt = Settings.SupportedAudioFormats.First(),
                Filter = CreateDialogFilter(),
                Multiselect = true
            };

            var result = chooseFiles.ShowDialog();

            if (result ?? true)
                AddToMediaList(chooseFiles.FileNames.ToArray());
        }

        #endregion

        #region Event Trigger Command Methods

        public bool MediaOpenedCommand_CanExecute()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && ModelMediaPlayer.SelectedMediaItem != null;
        }

        public void MediaOpenedCommand_Execute(object uiElement)
        {
            if (uiElement is MediaElement UI_MediaElement)
            {
                PollMediaPosition(UI_MediaElement);
            }
        }

        public bool SeekbarThumbCompletedDraggingCommand_CanExecute()
        {
            return ModelMediaPlayer.IsDraggingSeekbarThumb;
        }

        public void SeekbarThumbCompletedDraggingCommand_Execute(object uiElement)
        {
            if (uiElement is MediaElement UI_MediaElement)
            {
                ModelMediaPlayer.IsDraggingSeekbarThumb = false;
                UI_MediaElement.Position = TimeSpan.FromSeconds(ModelMediaPlayer.ElapsedTime);
            }
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
            if (uiElement is Slider UI_Seekbar)
            {
                if (ModelMediaPlayer.IsDraggingSeekbarThumb)
                {
                    ModelMediaPlayer.MediaPosition = TimeSpan.FromSeconds(UI_Seekbar.Value);
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddToMediaList(string[] files)
        {
            foreach (var file in files)
            {
                var id = ModelMediaPlayer.MediaList.Count > 0 ? ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.Last()) + 1 : 0;
                ModelMediaPlayer.MediaList.Add(_readMp3Metadata.GetMp3Metadata(id, file));
            }

            if (ModelMediaPlayer.SelectedMediaItem != null || ModelMediaPlayer.MediaList.Count <= 0)
                return;

            SelectMediaItem(ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.First()));
            PlayMedia();
        }

        #endregion

        #region Private Methods

        private string CreateDialogFilter()
        {
            string filter = "Supported Audio Formats (";

            filter += AppendSupportedAudioFormats(",");
            filter = filter.TrimEnd(',');
            filter += ") | ";
            filter += AppendSupportedAudioFormats(";");

            return filter;
        }

        private string AppendSupportedAudioFormats(string seperator)
        {
            return Settings.SupportedAudioFormats.Aggregate(string.Empty, (current, audioFormat) => current + $"*{audioFormat}{seperator}");
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
            return ModelMediaPlayer.MediaList.Count > 0 && ModelMediaPlayer.MediaList.Any(x => ModelMediaPlayer.MediaList.IndexOf(x) == ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) - 1);
        }

        private bool NextMediaItemIsAvailable()
        {
            return ModelMediaPlayer.MediaList.Count > 0 && ModelMediaPlayer.MediaList.Any(x => ModelMediaPlayer.MediaList.IndexOf(x) == ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) + 1);
        }

        private void PlayPreviousMediaItem()
        {
            if (ModelMediaPlayer.IsRepeatMediaListEnabled && ModelMediaPlayer.SelectedMediaItem?.Id == ModelMediaPlayer.MediaList.First().Id)
                SelectMediaItem(ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.Last()));
            else
                SelectMediaItem(ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.First(x => ModelMediaPlayer.MediaList.IndexOf(x) == ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) - 1)));

            PlayMedia();
        }

        private void PlayNextMediaItem()
        {
            if (ModelMediaPlayer.IsRepeatMediaListEnabled && ModelMediaPlayer.SelectedMediaItem?.Id == ModelMediaPlayer.MediaList.Last().Id)
                SelectMediaItem(ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.First()));
            else
                SelectMediaItem(ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.MediaList.First(x => ModelMediaPlayer.MediaList.IndexOf(x) == ModelMediaPlayer.MediaList.IndexOf(ModelMediaPlayer.SelectedMediaItem) + 1)));

            PlayMedia();
        }

        private void OrderMediaList()
        {
            ModelMediaPlayer.MediaList = new ObservableCollection<MediaItem>(ModelMediaPlayer.MediaList.OrderBy(x => x.Id));
        }

        private void ShuffleMediaList()
        {
            ModelMediaPlayer.MediaList = new ObservableCollection<MediaItem>(ModelMediaPlayer.MediaList
                .OrderBy(x => x != ModelMediaPlayer.SelectedMediaItem)
                .ThenBy(x => RandomIdGenerator.Next()));
        }

        private void PollMediaPosition(MediaElement mediaElement)
        {
            if (mediaElement != null)
            {
                MediaPositionTracker.Tick += (sender, args) => UpdateMediaPosition(mediaElement);
            }

            MediaPositionTracker.Start();
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

                CommandManager.InvalidateRequerySuggested();
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


        #endregion
     }
}
