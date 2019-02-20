using System.Windows;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Generic.Mediator;
using MediaPlayer.MVVM.Views;
using Ninject;

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
                Application.Current.MainWindow = iocKernal.Get<ViewApplicationSettings>();
                Application.Current.MainWindow.ShowDialog();
            });
        }
    }
}
