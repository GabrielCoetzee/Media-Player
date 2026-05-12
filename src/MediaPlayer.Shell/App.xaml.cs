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
using MediaPlayer.Settings.Config;
using MediaPlayer.Shell.MessengerRegs;
using MediaPlayer.View.Views;
using Velopack;
using Wpf.Ui.Appearance;

namespace MediaPlayer.Shell
{
    public partial class App : Application
    {
        private Mutex _mutex;
        private const string _mutexName = "MediaPlayer-{131763b7-57a3-4a9c-bbb5-97f2c86ba3c5}";
        public NamedPipeManager PipeManager { get; set; } = new NamedPipeManager("MediaPlayer");

        [Import]
        public ThemeSettings ThemeSettings { get; set; }

        protected async override void OnStartup(StartupEventArgs e)
        {
            VelopackApp.Build().Run();

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

            MessengerRegistrations.OpenMainWindow(MEF.Container);
            MessengerRegistrations.AddMedia(MEF.Container);
            MessengerRegistrations.SaveChangesToDirtyFiles(MEF.Container);
            MessengerRegistrations.AutoAdjustAccent(MEF.Container);

            LoadTheme(ThemeSettings.UseDarkMode);
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

                Messenger<MessengerMessages>.Send(MessengerMessages.AddMedia, args);
            });
        }

        private async Task SendArgsToFirstInstanceAsync(StartupEventArgs e)
        {
            await PipeManager.WriteLinesAsync(e.Args);
        }

        private static void LoadTheme(bool useDarkMode)
        {
            ApplicationThemeManager.Apply(useDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light);
        }

        private static void StartApplication(StartupEventArgs e)
        {
            Messenger<MessengerMessages>.Send(MessengerMessages.OpenMainWindow);

            if (!e.Args.Any())
                return;

            Messenger<MessengerMessages>.Send(MessengerMessages.AddMedia, e.Args);
        }

        private void InitializeMEF()
        {
            try
            {
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
