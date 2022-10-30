using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Metadata.Concrete.Readers;
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

        public MetadataLibraries MetadataLibrary => MetadataLibraries.Taglib;

        Func<string, bool> IsFolder = x => Directory.Exists(x);

        Func<string, bool> IsFile = x => File.Exists(x);

        public async Task<IEnumerable<MediaItem>> ReadFilePathsAsync(IEnumerable<string> filePaths)
        {
            var supportedFiles = new List<MediaItem>();

            await Task.Run(() =>
            {

                var metadataReader = _metadataReaderFactory.Resolve(MetadataLibrary);
                var supportedFileFormats = _applicationSettings.SupportedFileFormats;

                foreach (var file in SearchFolders(filePaths.Where(IsFolder), supportedFileFormats))
                    supportedFiles.Add(metadataReader.BuildMediaItem(file));

                foreach (var file in SearchFiles(filePaths.Where(IsFile), supportedFileFormats))
                    supportedFiles.Add(metadataReader.BuildMediaItem(file));
            });

            return supportedFiles;
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
