using System.Windows;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Generic.Mediator;
using Ninject;
using ViewApplicationSettings = MediaPlayer.View.Views.ViewApplicationSettings;
using ViewMediaPlayer = MediaPlayer.View.Views.ViewMediaPlayer;

namespace MediaPlayer.Shell.Mediator_Registrations
{
    public static class MediatorRegistrations
    {
        public static void RegisterOpenMediaPlayerMainWindow(IKernel iocKernal)
        {
            Mediator<MediatorMessages>.Register(MediatorMessages.OpenMediaPlayerMainWindow, (vm) =>
            {
                //ViewModel injected to ViewMediaPlayer Constructor using NInject IKernal
                Application.Current.MainWindow = iocKernal.Get<ViewMediaPlayer>();
                Application.Current.MainWindow.Show();
            });
        }

        public static void RegisterOpenApplicationSettingsWindow(IKernel iocKernal)
        {
            Mediator<MediatorMessages>.Register(MediatorMessages.OpenApplicationSettings, (vm) =>
            {
                //ViewModel injected to ViewApplicationSettings Constructor using NInject IKernal
                var window = iocKernal.Get<ViewApplicationSettings>();
                window.ShowDialog();
            });
        }
    }
}
