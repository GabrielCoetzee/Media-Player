using System.Windows;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Generic.Mediator;
using MediaPlayer.Shell.IoC;
using MediaPlayer.Shell.Mediator_Registrations;
using Ninject;

namespace MediaPlayer.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel _iocKernel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _iocKernel = new StandardKernel();
            _iocKernel.Load(new IocConfiguration());

            MediatorRegistrations.RegisterOpenMediaPlayerMainWindow(_iocKernel);
            MediatorRegistrations.RegisterOpenApplicationSettingsWindow(_iocKernel);

            Mediator<MediatorMessages>.NotifyColleagues(MediatorMessages.OpenMediaPlayerMainWindow, null);
        }
    }
}
