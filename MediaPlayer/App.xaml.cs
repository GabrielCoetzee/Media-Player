using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MediaPlayer.IoC;
using MediaPlayer.Properties;
using Ninject;
using Ninject.Modules;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel iocKernel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            iocKernel = new StandardKernel();
            iocKernel.Load(new IocConfiguration());

            Current.MainWindow = iocKernel.Get<ViewMediaPlayer>();
            Current.MainWindow.Show();
        }
    }
}
