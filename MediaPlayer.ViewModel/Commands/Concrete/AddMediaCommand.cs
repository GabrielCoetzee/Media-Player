using MediaPlayer.ApplicationSettings;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.ViewModel.Commands.Abstract;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    public class AddMediaCommand : IAddMediaCommand
    {
        readonly ISettingsProviderViewModel _settingsProvider;
        readonly MetadataReaderResolver _metadataReaderResolver;

        public AddMediaCommand(ISettingsProviderViewModel settingsProvider, 
            MetadataReaderResolver metadataReaderResolver)
        {
            _settingsProvider = settingsProvider;
            _metadataReaderResolver = metadataReaderResolver;
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
            if (parameter is not MainViewModel vm)
                return;

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

            vm.AddMediaItems(mediaItems);
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
