using System;
using System.Windows;
using Generic.Mediator;
using MediaPlayer.Common.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using ViewApplicationSettings = MediaPlayer.View.Views.ViewApplicationSettings;
using ViewMediaPlayer = MediaPlayer.View.Views.ViewMediaPlayer;

namespace MediaPlayer.Shell.MessengerRegs
{
    public static class MessengerRegistrations
    {
        public static void RegisterOpenMediaPlayerMainWindow(IServiceProvider serviceProvider)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.OpenMediaPlayerMainWindow, (vm) =>
            {
                serviceProvider.GetRequiredService<ViewMediaPlayer>().Show();
            });
        }

        public static void RegisterOpenApplicationSettingsWindow(IServiceProvider serviceProvider)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.OpenApplicationSettings, (vm) =>
            {
                serviceProvider.GetRequiredService<ViewApplicationSettings>().ShowDialog();
            });
        }
    }
}
