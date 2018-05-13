using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
        public MainWindow()
        {
            InitializeComponent();
            InitializeViewModel();

            this.AllowsTransparency = true;
        }

        private void InitializeViewModel()
        {
            DataContext = new ViewModelMediaPlayer();
        }

        private void SeekBar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var pointerLocation = (e.GetPosition(SeekBar).X / SeekBar.ActualWidth) * (SeekBar.Maximum - SeekBar.Minimum);
            SeekMediaPosition(TimeSpan.FromSeconds(pointerLocation));
        }

        private void SeekMediaPosition(TimeSpan seekToPosition)
        {
            this.MediaElement.Position = seekToPosition;
        }

        private void TopMostGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void TopMostGrid_Drop(object sender, DragEventArgs e)
        {
            if (!(DataContext is ViewModelMediaPlayer vm))
                return;

            var droppedContent = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            string directory = droppedContent;

            if (Path.HasExtension(droppedContent))
                directory = Path.GetDirectoryName(directory);

            if (!string.IsNullOrEmpty(directory))
            {
                var supportedFiles = Directory
                    .EnumerateFiles(directory)
                    .Where(file => vm.Settings.SupportedAudioFormats.Any(file.ToLower().EndsWith))
                    .ToList();

                vm.AddToMediaList(supportedFiles.ToArray());
            }
        }
        private void MediaListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MediaListBox.ScrollIntoView(MediaListBox.SelectedItem);
        }
    }
}
