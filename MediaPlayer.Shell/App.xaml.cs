using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Generic.DependencyInjection;
using Generic.Mediator;
using Generic.NamedPipes.Wrappers;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Settings;
using MediaPlayer.Settings.Config;
using MediaPlayer.Shell.MessengerRegs;
using MediaPlayer.View.Views;

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

        [Import]
        public ISettingsManager SettingsManager { get; set; }

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
            MessengerRegistrations.ProcessFilePaths(MEF.Container);
            MessengerRegistrations.SaveChangesToDirtyFiles(MEF.Container);

            LoadThemeResourceDictionary(SettingsManager.Accent);
            StartApplication(e);

            base.OnStartup(e);
        }

        public void FirstApplicationInstanceReceivedArguments(object sender, string[] args)
        {
            if (!args.Any())
                return;

            Dispatcher.Invoke(() =>
            {
                ((ViewMediaPlayer)Current.MainWindow).BringToForeground();

                Messenger<MessengerMessages>.Send(MessengerMessages.ProcessFilePaths, args);
            });
        }

        private async Task SendArgsToFirstInstanceAsync(StartupEventArgs e)
        {
            await PipeManager.WriteLinesAsync(e.Args);
        }

        private static void LoadThemeResourceDictionary(string accent)
        {
            var theme = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.{accent}.xaml", UriKind.RelativeOrAbsolute);

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = theme });
        }

        private static void StartApplication(StartupEventArgs e)
        {
            Messenger<MessengerMessages>.Send(MessengerMessages.OpenMediaPlayerMainWindow);

            if (!e.Args.Any())
                return;

            Messenger<MessengerMessages>.Send(MessengerMessages.ProcessFilePaths, e.Args);
        }

        private void InitializeMEF()
        {
            try
            {
                //MEF.Compose(Assembly.GetExecutingAssembly(), "MediaPlayer");
                MEF.ComposeAll(Assembly.GetExecutingAssembly());
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
