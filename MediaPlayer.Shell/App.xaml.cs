using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Generic.Configuration.Extensions;
using Generic.Mediator;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.ApplicationSettings.Concrete;
using MediaPlayer.ApplicationSettings.Config;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Metadata.Abstract;
using MediaPlayer.Model.Metadata.Concrete;
using MediaPlayer.Shell.MessengerRegs;
using MediaPlayer.Theming.Abstract;
using MediaPlayer.Theming.Concrete;
using MediaPlayer.View.Views;
using MediaPlayer.ViewModel;
using MediaPlayer.ViewModel.ViewModels;
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
        private IServiceProvider _serviceProvider;
        private IConfiguration _configuration;

        private Mutex _mutex;
        private const string _mutexName = "##||MediaPlayer||##";
        private const string _uniqueEventName = "9141e315-7f92-47d5-8460-8fc7fb7eb061";

        private readonly string _tempArgsPath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}//TempArgs.txt";
        readonly object _fileLock = new();

        public App()
        {
            CreateTempArgsFile();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, _mutexName, out var isFirstInstance);
            EventWaitHandle eventWaitHandle = new(false, EventResetMode.AutoReset, _uniqueEventName);

            if (!isFirstInstance)
            {
                lock (_fileLock)
                {
                    File.AppendAllLines(_tempArgsPath, e.Args);
                }

                eventWaitHandle.Set();
                Application.Current.Shutdown(0);
                return;
            }

            SetInitialInstanceThreadEventHandle(eventWaitHandle);

            var builder = new ConfigurationBuilder()
             .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
             .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            MessengerRegistrations.RegisterOpenMediaPlayerMainWindow(_serviceProvider);
            MessengerRegistrations.RegisterOpenApplicationSettingsWindow(_serviceProvider);

            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.OpenMediaPlayerMainWindow);
            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.ProcessContent, e.Args);

            base.OnStartup(e);
        }

        private void SetInitialInstanceThreadEventHandle(EventWaitHandle eventWaitHandle)
        {
            var thread = new Thread(() =>
            {
                while (eventWaitHandle.WaitOne())
                {
                    Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ((ViewMediaPlayer)Current.MainWindow).BringToForeground();

                        Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.ProcessContent, File.ReadAllLines(_tempArgsPath));

                        lock (_fileLock)
                        {
                            File.WriteAllText(_tempArgsPath, string.Empty);
                        }
                    }
                    ));
                }
            })
            {
                IsBackground = true
            };

            thread.Start();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureWritable<Settings>(_configuration.GetSection(nameof(Settings)));

            services.AddSingleton<ISettingsProviderViewModel, SettingsProviderViewModel>();
            services.AddTransient<IThemeSelector, ThemeSelector>();

            services.AddTransient<IMetadataReaderProvider, TaglibMetadataReaderProvider>();
            services.AddTransient<MetadataReaderResolver>();

            services.AddTransient(typeof(ViewMediaPlayer));
            services.AddTransient<MainViewModel>();
            services.AddTransient<BusyViewModel>();

            services.AddTransient(typeof(ViewApplicationSettings));
            services.AddTransient<ApplicationSettingsViewModel>();

            ViewModel.DependencyInjection.AddServices(services);
        }

        private void CreateTempArgsFile()
        {
            if (!File.Exists(_tempArgsPath))
                File.Create(_tempArgsPath);
        }
    }
}
