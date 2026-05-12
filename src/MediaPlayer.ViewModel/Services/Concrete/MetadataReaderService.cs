using MediaPlayer.Common.Constants;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Readers;
using MediaPlayer.Settings.Config;
using MediaPlayer.ViewModel.Services.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataReaderService))]
    public class MetadataReaderService : IMetadataReaderService
    {
        readonly IMetadataReader _metadataReader;
        readonly ApplicationSettings _applicationSettings;

        [ImportingConstructor]
        public MetadataReaderService([Import(ServiceNames.TaglibMetadataReader)] IMetadataReader metadataReader,
            ApplicationSettings applicationSettings)
        {
            _metadataReader = metadataReader;
            _applicationSettings = applicationSettings;
        }

        readonly Func<string, bool> IsFolder = x => Directory.Exists(x);

        readonly Func<string, bool> IsFile = x => File.Exists(x);

        public async IAsyncEnumerable<MediaItem> EnumerateMediaItemsAsync(IEnumerable<string> paths, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var supportedFileFormats = _applicationSettings.SupportedFileFormats;

            await foreach (var mediaItem in BuildMediaItemsAsync(SearchFolders(paths.Where(IsFolder), supportedFileFormats), cancellationToken))
                yield return mediaItem;

            await foreach (var mediaItem in BuildMediaItemsAsync(SearchFiles(paths.Where(IsFile), supportedFileFormats), cancellationToken))
                yield return mediaItem;
        }

        private async IAsyncEnumerable<MediaItem> BuildMediaItemsAsync(IEnumerable<string> files,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var mediaItem = await Task.Run(() => _metadataReader.BuildMediaItem(file), cancellationToken);

                if (mediaItem != null)
                    yield return mediaItem;
            }
        }

        private IEnumerable<string> SearchFolders(IEnumerable<string> folderPaths, string[] supportedFileFormats)
        {
            foreach (var path in folderPaths)
            {
                foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Where(file => supportedFileFormats.Any(file.ToLower().EndsWith)))
                    yield return file;
            }
        }

        private IEnumerable<string> SearchFiles(IEnumerable<string> filePaths, string[] supportedFileFormats)
        {
            foreach (var path in filePaths)
            {
                if (supportedFileFormats.Any(x => x.ToLower() == Path.GetExtension(path.ToLower())))
                    yield return path;
            }
        }
    }
}
