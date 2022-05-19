using Generic;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.Settings;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace MediaPlayer.ViewModel.Commands.Concrete
{
    [Export(CommandNames.AddMedia, typeof(ICommand))]
    public class AddMediaCommand : ICommand
    {
        readonly MetadataReaderResolver _metadataReaderResolver;

        [ImportingConstructor]
        public AddMediaCommand(MetadataReaderResolver metadataReaderResolver)
        {
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
                DefaultExt = vm.SettingsManager.SupportedFileFormats.First(),
                Filter = CreateDialogFilter(vm.SettingsManager.SupportedFileFormats),
                Multiselect = true
            };

            var result = chooseFiles.ShowDialog();

            if (result != DialogResult.OK)
                return;

            var metadataReader = _metadataReaderResolver.Resolve(MetadataReaders.Taglib);

            var mediaItems = chooseFiles.FileNames.Select(file => metadataReader.GetFileMetadata(file)).ToList();

            vm.AddMediaItems(mediaItems);
        }

        private string CreateDialogFilter(string[] supportedFileFormats)
        {
            return string.Join("|", $"Supported Formats ({AppendedSupportedFormats(",", supportedFileFormats)})", AppendedSupportedFormats(";", supportedFileFormats));
        }

        private string AppendedSupportedFormats(string seperator, string[] supportedFileFormats)
        {
            return supportedFileFormats.Aggregate(string.Empty, (current, format) => current + $"*{format}{(supportedFileFormats.Last() != format ? seperator : string.Empty)}");
        }
    }
}
