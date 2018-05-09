using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MediaPlayer.Helpers.Custom_Command_Classes;
using MediaPlayer.MetadataReaders.Factory;
using MediaPlayer.MetadataReaders.Interfaces;
using MediaPlayer.MetadataReaders.Types;
using MediaPlayer.MVVM.Models;
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
            ModelMediaPlayer = new ModelMediaPlayer() { TrackList = new ObservableCollection<Mp3>(), MediaState = MediaState.Pause, MediaVolume = Constants.VolumeLevel.FullVolume };
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
            return ModelMediaPlayer.CurrentTrack != null && ModelMediaPlayer.TrackList.Count > 0;
        }

        public void ClearMediaListCommand_Execute()
        {
            MediaPositionTracker.Stop();
            InitializeModelInstance();
        }

        public bool ShuffleMediaListCommand_CanExecute()
        {
            return ModelMediaPlayer.TrackList.Count > 2 && ModelMediaPlayer.CurrentTrack != null;
        }

        public void ShuffleMediaListCommand_Execute()
        {
            ShuffleMediaList();

            SelectMediaItem(ModelMediaPlayer.TrackList.First().Id);
            PlayMedia();
        }

        public bool RepeatMediaListCommand_CanExecute()
        {
            return ModelMediaPlayer.TrackList.Count > 0 && ModelMediaPlayer.CurrentTrack != null;
        }

        public void RepeatMediaListCommand_Execute()
        {
            ModelMediaPlayer.IsRepeatMediaListEnabled = !ModelMediaPlayer.IsRepeatMediaListEnabled;
        }

        public bool NextTrackCommand_CanExecute()
        {
            return ModelMediaPlayer.TrackList.Count > 0 && NextMediaItemIsAvailable() || ModelMediaPlayer.IsRepeatMediaListEnabled;
        }

        public void NextTrackCommand_Execute()
        {
            PlayNextMediaItem();
        }   

        public bool StopCommand_CanExecute()
        {
            return ModelMediaPlayer.TrackList.Count > 0 && ModelMediaPlayer.CurrentTrack != null;
        }

        public void StopCommand_Execute()
        {
            SelectMediaItem(ModelMediaPlayer.TrackList.First().Id);
            StopMedia();
        }

        public bool PreviousTrackCommand_CanExecute()
        {
            return ModelMediaPlayer.TrackList.Count > 0 && PreviousMediaItemIsAvailable() || ModelMediaPlayer.IsRepeatMediaListEnabled; 
        }

        public void PreviousTrackCommand_Execute()
        {
            PlayPreviousMediaItem();
        }

        public bool MuteCommand_CanExecute()
        {
            return ModelMediaPlayer.CurrentTrack != null;
        }

        public void MuteCommand_Execute()
        {
            ModelMediaPlayer.MediaVolume = ModelMediaPlayer.MediaVolume == Constants.VolumeLevel.FullVolume ? Constants.VolumeLevel.Mute : Constants.VolumeLevel.FullVolume;
        }

        public bool PlayPauseCommand_CanExecute()
        {
            return ModelMediaPlayer.CurrentTrack != null;
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
                DefaultExt = ".mp3",
                Filter = "MP3 Format|*.mp3",
                Multiselect = true
            };

            var result = chooseFiles.ShowDialog();

            if (result ?? true)
                AddToMediaList(chooseFiles.FileNames);
        }

        #endregion

        #region Event Trigger Command Methods

        public bool MediaOpenedCommand_CanExecute()
        {
            return ModelMediaPlayer.TrackList.Count > 0 && ModelMediaPlayer.CurrentTrack != null;
        }

        public void MediaOpenedCommand_Execute(object uiElement)
        {
            if (uiElement is MediaElement mediaElement)
            {
                PollMediaPosition(mediaElement);
            }
        }

        public bool SeekbarThumbCompletedDraggingCommand_CanExecute()
        {
            return ModelMediaPlayer.IsDraggingSeekbarThumb;
        }

        public void SeekbarThumbCompletedDraggingCommand_Execute(object uiElement)
        {
            if (uiElement is MediaElement mediaElement)
            {
                ModelMediaPlayer.IsDraggingSeekbarThumb = false;
                mediaElement.Position = TimeSpan.FromSeconds(ModelMediaPlayer.ElapsedTime);
            }
        }

        public bool SeekbarThumbStartedDraggingCommand_CanExecute()
        {
            return ModelMediaPlayer.CurrentTrack != null;
        }

        public void SeekbarThumbStartedCommand_Execute()
        {
            ModelMediaPlayer.IsDraggingSeekbarThumb = true;
        }

        public bool SeekbarValueChangedCommand_CanExecute()
        {
            return ModelMediaPlayer.CurrentTrack != null;
        }

        public void SeekbarValueChangedCommand_Execute(object uiElement)
        {
            if (uiElement is Slider seekbar)
            {
                if (ModelMediaPlayer.IsDraggingSeekbarThumb)
                {
                    ModelMediaPlayer.MediaPosition = TimeSpan.FromSeconds(seekbar.Value);
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddToMediaList(string[] files)
        {
            foreach (var file in files)
            {
                var id = ModelMediaPlayer.TrackList.Count > 0 ? ModelMediaPlayer.TrackList.Last().Id + 1 : 0;
                ModelMediaPlayer.TrackList.Add(_readMp3Metadata.GetMp3Metadata(id, file));
            }

            if (ModelMediaPlayer.CurrentTrack == null)
            {
                SelectMediaItem(ModelMediaPlayer.TrackList.First().Id);
                PlayMedia();
            }
        }

        #endregion

        #region Private Methods

        private void PlayMedia()
        {
            if (ModelMediaPlayer.MediaState != MediaState.Play)
                ModelMediaPlayer.MediaState = MediaState.Play;
        }

        private void PauseMedia()
        {
            if (ModelMediaPlayer.MediaState != MediaState.Pause)
                ModelMediaPlayer.MediaState = MediaState.Pause;
        }

        private void StopMedia()
        {
            if (ModelMediaPlayer.MediaState != MediaState.Stop)
                ModelMediaPlayer.MediaState = MediaState.Stop;
        }

        private void SelectMediaItem(int mediaItemId)
        {
            ModelMediaPlayer.CurrentTrack = ModelMediaPlayer.TrackList.First(x => x.Id == mediaItemId);
        }

        private bool PreviousMediaItemIsAvailable()
        {
            return ModelMediaPlayer.TrackList.Count > 0 && ModelMediaPlayer.TrackList.Any(x => x.Id == ModelMediaPlayer.CurrentTrack.Id - 1);
        }

        private bool NextMediaItemIsAvailable()
        {
            return ModelMediaPlayer.TrackList.Count > 0 && ModelMediaPlayer.TrackList.Any(x => x.Id == ModelMediaPlayer.CurrentTrack.Id + 1);
        }

        private void PlayPreviousMediaItem()
        {
            if (ModelMediaPlayer.IsRepeatMediaListEnabled && ModelMediaPlayer.CurrentTrack.Id == ModelMediaPlayer.TrackList.First().Id)
                SelectMediaItem(ModelMediaPlayer.TrackList.Last().Id);
            else
                SelectMediaItem(ModelMediaPlayer.TrackList.First(x => x.Id == ModelMediaPlayer.CurrentTrack.Id - 1).Id);

            PlayMedia();
        }

        private void PlayNextMediaItem()
        {
            if (ModelMediaPlayer.IsRepeatMediaListEnabled && ModelMediaPlayer.CurrentTrack.Id == ModelMediaPlayer.TrackList.Last().Id)
                SelectMediaItem(ModelMediaPlayer.TrackList.First().Id);
            else
                SelectMediaItem(ModelMediaPlayer.TrackList.First(x => x.Id == ModelMediaPlayer.CurrentTrack.Id + 1).Id);

            PlayMedia();
        }

        //private List<int> GetShuffledMediaListIds()
        //{
        //    return ModelMediaPlayer.TrackList
        //        .Where(x => x.Id != ModelMediaPlayer.CurrentTrack.Id)
        //        .OrderBy(x => RandomIdGenerator.Next())
        //        .Select(x => x.Id)
        //        .ToList();
        //}

        private void ShuffleMediaList()
        {
            ModelMediaPlayer.TrackList = new ObservableCollection<Mp3>(ModelMediaPlayer.TrackList.OrderBy(x => RandomIdGenerator.Next()));

            ModelMediaPlayer.TrackList.Move(ModelMediaPlayer.TrackList.IndexOf(ModelMediaPlayer.TrackList.First(x => x.Id == ModelMediaPlayer.CurrentTrack.Id)), 0);

            ModelMediaPlayer.TrackList.ToList().ForEach(x => x.Id = ModelMediaPlayer.TrackList.IndexOf(x));
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
                    NextTrackCommand_Execute();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void SetAccurateCurrentMediaDuration(TimeSpan mediaDuration)
        {
            ModelMediaPlayer.CurrentTrack.TrackDuration = mediaDuration;
        }

        private bool IsEndOfCurrentMedia(MediaElement mediaElement)
        {
            return mediaElement.Position.TotalSeconds == ModelMediaPlayer.CurrentTrack.TrackDuration.TotalSeconds;
        }

        private bool CurrentMediaDurationIsAccurate(MediaElement mediaElement)
        {
            return ModelMediaPlayer.CurrentTrack.TrackDuration == mediaElement.NaturalDuration.TimeSpan;
        }


        #endregion
     }
}
