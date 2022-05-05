using System;
using System.IO;
using System.Windows;
using Generic.Configuration.Extensions;
using Generic.Mediator;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.ApplicationSettings.Config;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model.Abstract;
using MediaPlayer.Model.Implementation;
using MediaPlayer.Shell.MessengerRegs;
using MediaPlayer.Theming;
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
        private IServiceProvider _serviceProvider { get; set; }
        private IConfiguration _configuration { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            MessengerRegistrations.RegisterOpenMediaPlayerMainWindow(_serviceProvider);
            MessengerRegistrations.RegisterOpenApplicationSettingsWindow(_serviceProvider);

            Messenger<MessengerMessages>.NotifyColleagues(MessengerMessages.OpenMediaPlayerMainWindow);

            base.OnStartup(e);
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
    }
}
