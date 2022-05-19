using System;
using System.Linq;
using Generic;
using Generic.Configuration.Abstract;
using Generic.Mediator;
using MediaPlayer.Common.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using ViewApplicationSettings = MediaPlayer.View.Views.ViewApplicationSettings;
using ViewMediaPlayer = MediaPlayer.View.Views.ViewMediaPlayer;

namespace MediaPlayer.Shell.MessengerRegs
{
    public class MessengerRegistrations
    {
        public static void OpenMediaPlayerMainWindow(IServiceProvider serviceProvider)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.OpenMediaPlayerMainWindow, (args) =>
            {
                var view = MEF.Container?.GetExports<ViewMediaPlayer>().SingleOrDefault().Value;

                view.Show();

                //serviceProvider.GetRequiredService<ViewMediaPlayer>().Show();
            });
        }

        public static void OpenApplicationSettingsWindow(IServiceProvider serviceProvider)
        {
            Messenger<MessengerMessages>.Register(MessengerMessages.OpenApplicationSettings, (args) =>
            {
                var view = MEF.Container?.GetExports<ViewApplicationSettings>().SingleOrDefault().Value;

                view.ShowDialog();

                //serviceProvider.GetRequiredService<ViewApplicationSettings>().ShowDialog();
            });
        }
    }
}
