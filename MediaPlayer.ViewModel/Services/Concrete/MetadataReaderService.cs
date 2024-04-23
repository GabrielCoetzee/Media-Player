using MediaPlayer.Common.Constants;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Abstract.Correctors;
using MediaPlayer.Model.Metadata.Abstract.Readers;
using MediaPlayer.Settings.Config;
using MediaPlayer.ViewModel.Services.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel.Services.Concrete
{
    [Export(typeof(IMetadataReaderService))]
    public class MetadataReaderService : IMetadataReaderService
    {
        readonly IMetadataReader _metadataReader;
        readonly ApplicationSettings _applicationSettings;
        readonly IEnumerable<IMetadataCorrector> _metadataCorrectors;

        [ImportingConstructor]
        public MetadataReaderService([Import(ServiceNames.TaglibMetadataReader)] IMetadataReader metadataReader,
            ApplicationSettings applicationSettings,
            [ImportMany] IEnumerable<IMetadataCorrector> metadataCorrectors)
        {
            _metadataReader = metadataReader;
            _applicationSettings = applicationSettings;
            _metadataCorrectors = metadataCorrectors;
        }

        readonly Func<string, bool> IsFolder = x => Directory.Exists(x);

        readonly Func<string, bool> IsFile = x => File.Exists(x);

        public async Task<IEnumerable<MediaItem>> ReadFilePathsAsync(IEnumerable<string> filePaths)
        {
            var mediaItems = new List<MediaItem>();

            await Task.Run(() =>
            {
                var supportedFileFormats = _applicationSettings.SupportedFileFormats;

                foreach (var file in SearchFolders(filePaths.Where(IsFolder), supportedFileFormats))
                    mediaItems.Add(_metadataReader.BuildMediaItem(file));

                foreach (var file in SearchFiles(filePaths.Where(IsFile), supportedFileFormats))
                    mediaItems.Add(_metadataReader.BuildMediaItem(file));

                foreach (var mediaItem in mediaItems)
                    _metadataCorrectors.Where(x => x.IsValid(mediaItem)).ToList().ForEach(x => x.FixMetadata(mediaItem));

            });

            return mediaItems.Where(x => x != null);
        }

        private IEnumerable<string> SearchFolders(IEnumerable<string> filePaths, string[] supportedFileFormats)
        {
            foreach (var path in filePaths)
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
