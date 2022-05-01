using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Generic.Configuration.Extensions;
using Generic.Mediator;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.ApplicationSettings.Config;
using MediaPlayer.BusinessLogic;
using MediaPlayer.BusinessLogic.Abstract;
using MediaPlayer.BusinessLogic.Commands;
using MediaPlayer.BusinessLogic.Commands.Abstract;
using MediaPlayer.BusinessLogic.Commands.Concrete;
using MediaPlayer.BusinessLogic.Implementation;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Model;
using MediaPlayer.Shell.MessengerRegs;
using MediaPlayer.Theming;
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

            services.AddSingleton<ISettingsProvider, SettingsProvider>();
            services.AddSingleton<IThemeSelector, ThemeSelector>();

            services.AddTransient<IMetadataReaderProvider, TaglibMetadataReaderProvider>();
            services.AddTransient<MetadataReaderResolver>();

            services.AddTransient(typeof(ViewMediaPlayer));
            services.AddTransient<ViewModelMediaPlayer>();

            services.AddTransient(typeof(ViewApplicationSettings));
            services.AddTransient<ViewModelApplicationSettings>();

            services.AddSingleton<ModelMediaPlayer>();

            //Commands
            services.AddTransient<IOpenSettingsWindowCommand, OpenSettingsWindowCommand>();
            services.AddTransient<IShuffleCommand, ShuffleCommand>();
            services.AddTransient<IAddMediaCommand, AddMediaCommand>();
            services.AddTransient<IPlayPauseCommand, PlayPauseCommand>();
            services.AddTransient<IMuteCommand, MuteCommand>();
            services.AddTransient<IPreviousTrackCommand, PreviousTrackCommand>();
            services.AddTransient<IStopCommand, StopCommand>();
            services.AddTransient<INextTrackCommand, NextTrackCommand>();
            services.AddTransient<IRepeatMediaListCommand, RepeatMediaListCommand>();
            services.AddTransient<IMediaOpenedCommand, MediaOpenedCommand>();
            services.AddTransient<IClearMediaListCommand, ClearMediaListCommand>();
            services.AddTransient<ISeekbarValueChangedCommand, SeekbarValueChangedCommand>();
            services.AddTransient<ISeekbarThumbStartedDraggingCommand, SeekbarThumbStartedDraggingCommand>();
            services.AddTransient<ISeekbarThumbCompletedDraggingCommand, SeekbarThumbCompletedDraggingCommand>();
        }
    }
}
