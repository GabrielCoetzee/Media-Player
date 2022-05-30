using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Generic;
using Generic.Configuration.Concrete;
using Generic.Configuration.Extensions;
using Generic.DependencyInjection;
using Generic.Mediator;
using Generic.NamedPipes.Wrappers;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.Settings;
using MediaPlayer.Settings.Concrete;
using MediaPlayer.Shell.MessengerRegs;
using MediaPlayer.Theming.Abstract;
using MediaPlayer.Theming.Concrete;
using MediaPlayer.View.Views;
using MediaPlayer.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaPlayer.Shell
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;
        private const string _mutexName = "##||MediaPlayer||##";
        public NamedPipeManager PipeManager { get; set; } = new NamedPipeManager("MediaPlayer");

        public void FirstApplicationInstanceReceivedArguments(object sender, string[] args)
        {
            if (!args.Any())
                return;

            Dispatcher.Invoke(() =>
            {
                ((ViewMediaPlayer)Current.MainWindow).BringToForeground();

                Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.ProcessFilePaths, args);
            });
        }

        private async Task SendArgsToFirstInstanceAsync(StartupEventArgs e)
        {
            await PipeManager.WriteLinesAsync(e.Args);
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, _mutexName, out var isFirstInstance);

            if (!isFirstInstance)
            {
                await SendArgsToFirstInstanceAsync(e);

                Application.Current.Shutdown(0);
                return;
            }

            PipeManager.StartServer();
            PipeManager.ServerReceivedArguments += FirstApplicationInstanceReceivedArguments;

            InitializeMEF();

            MessengerRegistrations.OpenMediaPlayerMainWindow(MEF.Container);
            MessengerRegistrations.OpenApplicationSettingsWindow(MEF.Container);
            MessengerRegistrations.ProcessContent(MEF.Container);

            StartApplication(e);

            base.OnStartup(e);
        }

        private static void StartApplication(StartupEventArgs e)
        {
            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.OpenMediaPlayerMainWindow);

            if (!e.Args.Any())
                return;

            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.ProcessFilePaths, e.Args);
        }

        private void InitializeMEF()
        {
            try
            {
                MEF.Compose(Assembly.GetExecutingAssembly(), "MediaPlayer");
                MEF.Build(this);
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var exception in ex.LoaderExceptions)
                {
                    MessageBox.Show(exception.Message, ex.GetType().ToString());
                }
            }
        }
    }
}
