using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MediaPlayer.Application_Settings.Interface_Implementations;
using MediaPlayer.MetadataReaders.Interfaces;
using MediaPlayer.MVVM.Models.Base_Types;
using MediaPlayer.MVVM.ViewModels;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ViewMediaPlayer : MetroWindow
    {
        #region Properties

        private BackgroundWorker mediaListProcessor_background;
        
        #endregion

        #region Constructor

        public ViewMediaPlayer()
        {
            InitializeComponent();

            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));

            InitializeViewModel();
            InitializeMediaListProcessor();

            this.AllowsTransparency = true;
        }

        #endregion

        #region Initialization

        private void InitializeViewModel()
        {
            DataContext = new ViewModelMediaPlayer();
        }

        private void InitializeMediaListProcessor()
        {
            mediaListProcessor_background = new BackgroundWorker();

            mediaListProcessor_background.DoWork += MediaListProcessorProcessDroppedContent;
            mediaListProcessor_background.RunWorkerCompleted += MediaListProcessorCompleted;
        }

        #endregion

        #region UI Event Handlers

        private void SeekBar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var pointerLocation = (e.GetPosition(SeekBar).X / SeekBar.ActualWidth) * (SeekBar.Maximum - SeekBar.Minimum);
            SeekMediaPosition(TimeSpan.FromSeconds(pointerLocation));
        }

        private void TopMostGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void TopMostGrid_Drop(object sender, DragEventArgs e)
        {
            var droppedContent = ((IEnumerable)e.Data.GetData(DataFormats.FileDrop));

            if (droppedContent == null)
                return;

            ProcessDroppedFilesAndFolders(droppedContent);
        }
    
        private void ProcessDroppedFilesAndFolders(IEnumerable droppedContent)
        {
            if (!(DataContext is ViewModelMediaPlayer vm))
                return;

            vm.ModelMediaPlayer.IsLoadingMediaItems = true;

            mediaListProcessor_background.RunWorkerAsync(new MediaItemProcessingArguments() { FilePaths = droppedContent, ReadMetadata = vm.MetadataReader });
        }

        private void MediaListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MediaListBox.ScrollIntoView(MediaListBox.SelectedItem);
            FocusOnPlayPauseButton();
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            if (!(DataContext is ViewModelMediaPlayer vm))
                return;

            LoadTheme(vm.ApplicationSettings.SelectedTheme);
        }

        private void LyricsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            FocusOnPlayPauseButton();
        }

        private void LyricsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            FocusOnPlayPauseButton();
        }

        #endregion

        #region Private Methods

        private void SeekMediaPosition(TimeSpan seekToPosition)
        {
            this.MediaElement.Position = seekToPosition;
        }


        private void LoadTheme(string accentName)
        {
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(accentName), ThemeManager.GetAppTheme("BaseDark"));
        }

        private void FocusOnPlayPauseButton()
        {
            ButtonPlayPause.Focus();
        }

        #endregion

        #region Background Thread - File Processing

        private void MediaListProcessorProcessDroppedContent(object o, DoWorkEventArgs args)
        {
            if (!(args.Argument is MediaItemProcessingArguments mediaItemArgs))
                return;

            var supportedFiles = new List<MediaItem>();

            foreach (var path in mediaItemArgs.FilePaths)
            {
                bool isFolder = !Path.HasExtension(path.ToString());

                if (isFolder)
                {
                    supportedFiles.AddRange(Directory.EnumerateFiles(path.ToString(), "*.*", SearchOption.AllDirectories)
                        .Where(file => ApplicationSettings.Instance.SupportedFormats.Any(file.ToLower().EndsWith))
                        .Select((x) =>  mediaItemArgs.ReadMetadata.GetFileMetadata(x))
                        .ToList());
                }
                else
                {
                    if (ApplicationSettings.Instance.SupportedFormats.Any(x => x.ToLower().Equals(Path.GetExtension(path.ToString().ToLower()))))
                        supportedFiles.Add(mediaItemArgs.ReadMetadata.GetFileMetadata(path.ToString()));
                }

            }

            args.Result = supportedFiles;
        }

        private void MediaListProcessorCompleted(object o, RunWorkerCompletedEventArgs args)
        {
            if (!(DataContext is ViewModelMediaPlayer vm))
                return;

            if (!(args.Result is List<MediaItem> mediaItems))
                return;

            vm.AddToMediaList(mediaItems);
            vm.ModelMediaPlayer.IsLoadingMediaItems = false;
        }

        #endregion

        private void Button_OnLoaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == null)
                return;

            button.Height = button.ActualHeight;
            button.Width = button.ActualWidth;
        }

        private void ToggleButton_OnLoaded(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;

            if (button == null)
                return;

            button.Height = button.ActualHeight;
            button.Width = button.ActualWidth;
        }
    }

    public class MediaItemProcessingArguments
    {
        public IReadMetadata ReadMetadata { get; set; }

        public IEnumerable FilePaths { get; set; }
    }
}
