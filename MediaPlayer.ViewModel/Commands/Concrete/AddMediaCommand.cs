using Generic.Mediator;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Settings.Config;
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
        readonly ApplicationSettings _applicationSettings;

        [ImportingConstructor]
        public AddMediaCommand(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
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
                DefaultExt = _applicationSettings.SupportedFileFormats.First(),
                Filter = CreateDialogFilter(_applicationSettings.SupportedFileFormats),
                Multiselect = true
            };

            var result = chooseFiles.ShowDialog();

            if (result != DialogResult.OK)
                return;

            Messenger<MessengerMessages>.Send(MessengerMessages.ProcessFilePaths, chooseFiles.FileNames);
        }

        private string CreateDialogFilter(string[] supportedFileFormats)
        {
            return string.Join("|", $"Supported Formats ({AggregatedSupportedExtensions(",", supportedFileFormats)})", AggregatedSupportedExtensions(";", supportedFileFormats));
        }

        private string AggregatedSupportedExtensions(string seperator, string[] supportedFileFormats)
        {
            return supportedFileFormats.Aggregate(string.Empty, (seed, extension) => seed + $"*{extension}{(supportedFileFormats.Last() != extension ? seperator : string.Empty)}");
        }
    }
}
