using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MediaPlayer.MVVM.Models.Objects;
using MediaPlayer.MVVM.ViewModels;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            InitializeViewModel();

            this.AllowsTransparency = true;
        }

        #endregion

        #region Initialization

        private void InitializeViewModel()
        {
            DataContext = new ViewModelMediaPlayer();
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
            if (!(DataContext is ViewModelMediaPlayer vm))
                return;

            var droppedContent = ((string[])e.Data.GetData(DataFormats.FileDrop));

            if (droppedContent == null)
                return;

            var supportedFiles = new List<string>();

            foreach (var path in droppedContent)
            {
                if (!string.IsNullOrWhiteSpace(path) && !Path.HasExtension(path))
                {
                    supportedFiles.AddRange(Directory
                        .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                        .Where(file => vm.ApplicationSettings.SupportedAudioFormats.Any(file.ToLower().EndsWith))
                        .ToList());

                }
                else if (!string.IsNullOrWhiteSpace(path) && Path.HasExtension(path))
                {
                    if (vm.ApplicationSettings.SupportedAudioFormats.Any(x => x.ToLower().Equals(Path.GetExtension(path.ToLower()))))
                        supportedFiles.Add(path);
                }
            }

            vm.AddToMediaList(supportedFiles.ToArray());
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
    }
}
