﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings.Interfaces;
using MediaPlayer.BusinessEntities.Objects.Abstract;
using MediaPlayer.MetadataReaders;
using MediaPlayer.ViewModel;
using Ninject;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ViewMediaPlayer : MetroWindow
    {
        #region Injected Properties

        [Inject]
        public IExposeApplicationSettings ApplicationSettings { get; set; }

        [Inject]
        public MetadataReaderProviderResolver MetadataReaderProviderResolver { get; set; }

        #endregion

        #region Constructor

        [Inject]
        public ViewMediaPlayer(ViewModelMediaPlayer vm)
        {
            this.InitializeComponent();
            this.InitializeViewModel(vm);

            this.AllowsTransparency = true;
        }

        #endregion

        #region Initialization

        private void InitializeViewModel(ViewModelMediaPlayer vm)
        {
            DataContext = vm;
        }

        #endregion

        #region UI Event Handlers

        private void SeekBar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var pointerLocation = (e.GetPosition(this.SeekBar).X / SeekBar.ActualWidth) * (SeekBar.Maximum - SeekBar.Minimum);

            this.SeekMediaPosition(TimeSpan.FromSeconds(pointerLocation));
        }

        private void TopMostGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private async void TopMostGrid_Drop(object sender, DragEventArgs e)
        {
            if (!(DataContext is ViewModelMediaPlayer vm))
                return;

            var droppedContent = ((IEnumerable)e.Data.GetData(DataFormats.FileDrop));

            if (droppedContent == null)
                return;

            vm.SetIsLoadingMediaItems(true);

            var mediaItems = await ProcessDroppedContentAsync(droppedContent);

            vm.AddToMediaList(mediaItems);

            vm.SetIsLoadingMediaItems(false);

        }

        private void MediaListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MediaListBox.ScrollIntoView(MediaListBox.SelectedItem);

            this.FocusOnPlayPauseButton();
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            this.LoadTheme(ApplicationSettings.SelectedTheme);
        }

        private void LyricsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.FocusOnPlayPauseButton();
        }

        private void LyricsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            this.FocusOnPlayPauseButton();
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

        private async Task<IEnumerable<MediaItem>> ProcessDroppedContentAsync(IEnumerable filePaths)
        {
            var supportedFiles = new List<MediaItem>();

            await Task.Run(() =>
            {
                var metadataReader = MetadataReaderProviderResolver.Resolve(Common.Enumerations.MetadataReaders.Taglib);

                foreach (var path in filePaths)
                {
                    var isFolder = !Path.HasExtension(path.ToString());

                    if (isFolder)
                    {
                        supportedFiles.AddRange(Directory
                            .EnumerateFiles(path.ToString(), "*.*", SearchOption.AllDirectories)
                            .Where(file => ApplicationSettings.SupportedFormats.Any(file.ToLower().EndsWith))
                            .Select((x) => metadataReader.GetFileMetadata(x))
                            .ToList());
                    }
                    else
                    {
                        if (ApplicationSettings.SupportedFormats.Any(x =>
                            x.ToLower().Equals(Path.GetExtension(path.ToString().ToLower()))))
                            supportedFiles.Add(metadataReader.GetFileMetadata(path.ToString()));
                    }

                }
            });

            return supportedFiles;
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
}
