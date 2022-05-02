using MediaPlayer.ApplicationSettings;
using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.Services.Abstract;
using MediaPlayer.Common.Enumerations;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace MediaPlayer.BusinessLogic.Commands.Concrete
{
    public class AddMediaCommand : IAddMediaCommand
    {
        readonly ISettingsProvider _settingsProvider;
        readonly MetadataReaderResolver _metadataReaderResolver;
        readonly IMediaListService _mediaListService;

        public AddMediaCommand(ISettingsProvider settingsProvider, 
            MetadataReaderResolver metadataReaderResolver,
            IMediaListService mediaListService)
        {
            _settingsProvider = settingsProvider;
            _metadataReaderResolver = metadataReaderResolver;
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

        public void Execute(object parameter)
        {
            var chooseFiles = new OpenFileDialog
            {
                Title = "Choose Files",
                DefaultExt = _settingsProvider.SupportedFileFormats.First(),
                Filter = CreateDialogFilter(),
                Multiselect = true
            };

            var result = chooseFiles.ShowDialog();

            if (result != DialogResult.OK)
                return;

            var metadataReader = _metadataReaderResolver.Resolve(MetadataReaders.Taglib);

            var mediaItems = chooseFiles.FileNames.Select(file => metadataReader.GetFileMetadata(file)).ToList();

            _mediaListService.AddRange(mediaItems);
        }

        private string CreateDialogFilter()
        {
            return string.Join("|", $"Supported Formats ({AppendedSupportedFormats(",")})", AppendedSupportedFormats(";"));
        }

        private string AppendedSupportedFormats(string seperator)
        {
            return _settingsProvider.SupportedFileFormats.Aggregate(string.Empty, (current, format) => current + $"*{format}{(_settingsProvider.SupportedFileFormats.Last() != format ? seperator : string.Empty)}");
        }
    }
}
