using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
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

        public void FirstApplicationInstanceReceivedArguments(object sender, string args)
        {
            if (string.IsNullOrEmpty(args))
                return;

            Dispatcher.Invoke(() =>
            {
                ((ViewMediaPlayer)Current.MainWindow).BringToForeground();

                var filePaths = args.ToString().Split(Environment.NewLine.ToCharArray());

                Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.ProcessContent, filePaths);
            });
        }

        private void SendArgsToFirstInstance(StartupEventArgs e)
        {
            StringBuilder sb = new();

            foreach (var arg in e.Args)
                sb.AppendLine(arg);

            PipeManager.Write(sb.ToString());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, _mutexName, out var isFirstInstance);

            if (!isFirstInstance)
            {
                SendArgsToFirstInstance(e);

                Application.Current.Shutdown(0);
                return;
            }

            PipeManager.StartServer();
            PipeManager.ServerReceivedArgument += FirstApplicationInstanceReceivedArguments;

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
            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.ProcessContent, e.Args);
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
