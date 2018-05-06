using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
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
            if (DataContext is ViewModelMediaPlayer vm)
            {
                var pointerLocation = (e.GetPosition(SeekBar).X / SeekBar.ActualWidth) * (SeekBar.Maximum - SeekBar.Minimum);
                SeekMediaPosition(TimeSpan.FromSeconds(pointerLocation));
            }
        }

        private void SeekMediaPosition(TimeSpan seekToPosition)
        {
            this.MediaElement.Position = seekToPosition;
        }

        private void TopMostGrid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TopMostGrid_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is ViewModelMediaPlayer vm)
            {
                var droppedContent = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                string directory = droppedContent;

                if (Path.HasExtension(droppedContent))
                    directory = Path.GetDirectoryName(directory);

                if (!string.IsNullOrEmpty(directory))
                {
                    var filteredFiles = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase));
                    vm.AddToMediaList(filteredFiles.ToArray());
                }
            }
        }
        private void MediaListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MediaListBox.ScrollIntoView(MediaListBox.SelectedItem);
        }
    }
}
