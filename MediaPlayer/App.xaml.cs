using System.Windows;
using MediaPlayer.IoC;
using Ninject;

namespace MediaPlayer
{
    /// <inheritdoc />
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

            Current.MainWindow = _iocKernel.Get<MVVM.Views.ViewMediaPlayer>();
            Current.MainWindow.Show();
        }
    }
}
