using System;
using System.Linq;
using Generic;
using Generic.Mediator;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.View.Views;

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
