using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.BusinessLogic;
using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Theming;
using MediaPlayer.ViewModel;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ViewMediaPlayer : MetroWindow
    {
        readonly ISettingsProvider _settingsProvider;
        readonly IThemeSelector _themeSelector;
        readonly MetadataReaderResolver _metadataReaderResolver;

        public ViewMediaPlayer(ViewModelMediaPlayer vm, 
            ISettingsProvider settingsProvider, 
            IThemeSelector themeSelector, 
            MetadataReaderResolver metadataReaderResolver)
        {
            InitializeComponent();

            DataContext = vm;

            this._settingsProvider = settingsProvider;
            this._themeSelector = themeSelector;
            this._metadataReaderResolver = metadataReaderResolver;
            this.AllowsTransparency = true;
        }

        #region UI Event Handlers

        private void TopMostGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private async void TopMostGrid_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is not ViewModelMediaPlayer vm)
                return;

            var droppedContent = (IEnumerable)e.Data.GetData(DataFormats.FileDrop);

            if (droppedContent == null)
                return;

            vm.SetIsLoadingMediaItems(true);

            var mediaItems = await ProcessDroppedContentAsync(droppedContent);

            vm.AddToMediaList(mediaItems);

            vm.SetIsLoadingMediaItems(false);
        }

        private void MediaListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.FocusOnPlayPauseButton();
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            this.LoadTheme();
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

        private void LoadTheme()
        {
            this._themeSelector.ChangeAccent(this._settingsProvider.SelectedAccent);
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
                var metadataReader = _metadataReaderResolver.Resolve(MetadataReaders.Taglib);
                var supportedFileFormats = this._settingsProvider.SupportedFileFormats;

                foreach (var path in filePaths)
                {
                    var isFolder = Directory.Exists(path.ToString());

                    if (isFolder)
                    {
                        supportedFiles.AddRange(Directory
                            .EnumerateFiles(path.ToString(), "*.*", SearchOption.AllDirectories)
                            .Where(file => supportedFileFormats.Any(file.ToLower().EndsWith))
                            .Select((x) => metadataReader.GetFileMetadata(x)));
                    }
                    else
                    {
                        if (supportedFileFormats.Any(x => x.ToLower() == Path.GetExtension(path.ToString().ToLower())))
                        {
                            supportedFiles.Add(metadataReader.GetFileMetadata(path.ToString()));
                        }
                    }

                }
            });

            return supportedFiles;
        }

        #endregion
    }
}
