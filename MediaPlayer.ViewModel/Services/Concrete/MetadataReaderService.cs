using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.Settings.Config;
using MediaPlayer.ViewModel.Services.Abstract;
using System;
using System.Collections;
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
        readonly MetadataReaderFactory _metadataReaderFactory;
        readonly ApplicationSettings _applicationSettings;

        [ImportingConstructor]
        public MetadataReaderService(MetadataReaderFactory metadataReaderFactory,
            ApplicationSettings applicationSettings)
        {
            _metadataReaderFactory = metadataReaderFactory;
            _applicationSettings = applicationSettings;
        }

        public async Task<IEnumerable<MediaItem>> ReadFilePathsAsync(IEnumerable filePaths)
        {
            var supportedFiles = new List<MediaItem>();

            await Task.Run(() =>
            {
                var metadataReader = _metadataReaderFactory.Resolve(MetadataLibraries.Taglib);
                var supportedFileFormats = _applicationSettings.SupportedFileFormats;

                foreach (var path in filePaths)
                {
                    var isFolder = Directory.Exists(path.ToString());

                    if (isFolder)
                    {
                        var mediaItems = Directory
                            .EnumerateFiles(path.ToString(), "*.*", SearchOption.AllDirectories)
                            .Where(file => supportedFileFormats.Any(file.ToLower().EndsWith))
                            .Select((x) => metadataReader.BuildMediaItem(x));

                        supportedFiles.AddRange(mediaItems);
                    }
                    else
                    {
                        if (supportedFileFormats.Any(x => x.ToLower() == Path.GetExtension(path.ToString().ToLower())))
                        {
                            var mediaItem = metadataReader.BuildMediaItem(path.ToString());

                            supportedFiles.Add(mediaItem);
                        }
                    }

                }
            });

            return supportedFiles;
        }
    }
}
