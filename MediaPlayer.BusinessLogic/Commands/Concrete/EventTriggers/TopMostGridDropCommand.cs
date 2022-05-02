using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessEntities.Objects.Base;
using MediaPlayer.BusinessLogic.Commands.Abstract.EventTriggers;
using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete.EventTriggers
{
    public class TopMostGridDropCommand : ITopMostGridDropCommand
    {
        readonly ModelMediaPlayer _model;
        readonly MetadataReaderResolver _metadataReaderResolver;
        readonly ISettingsProvider _settingsProvider;
        readonly IMediaListService _mediaListService;

        public TopMostGridDropCommand(ModelMediaPlayer model, 
            MetadataReaderResolver metadataReaderResolver, 
            ISettingsProvider settingsProvider,
            IMediaListService mediaListService)
        {
            _model = model;
            _metadataReaderResolver = metadataReaderResolver;
            _settingsProvider = settingsProvider;
            _mediaListService = mediaListService;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            if (parameter is not DragEventArgs e)
                return;

            var droppedContent = (IEnumerable)e.Data.GetData(DataFormats.FileDrop);

            if (droppedContent == null)
                return;

            _model.IsLoadingMediaItems = true;

            var mediaItems = await ProcessDroppedContentAsync(droppedContent);

            _mediaListService.AddRange(mediaItems);

            _model.IsLoadingMediaItems = false;
        }

        private async Task<IEnumerable<MediaItem>> ProcessDroppedContentAsync(IEnumerable filePaths)
        {
            var supportedFiles = new List<MediaItem>();

            await Task.Run(() =>
            {
                var metadataReader = _metadataReaderResolver.Resolve(MetadataReaders.Taglib);
                var supportedFileFormats = _settingsProvider.SupportedFileFormats;

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
    }
}
