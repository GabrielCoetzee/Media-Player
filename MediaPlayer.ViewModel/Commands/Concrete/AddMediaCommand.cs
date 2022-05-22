using Generic.Mediator;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Settings.Config;
using MediaPlayer.ViewModel.Services.Abstract;
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

        public async void Execute(object parameter)
        {
            if (parameter is not MainViewModel vm)
                return;

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

            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.ProcessContent, chooseFiles.FileNames);
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
