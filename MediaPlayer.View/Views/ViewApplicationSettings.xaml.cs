using System.ComponentModel;
using System.Linq;
using System.Windows;
using ControlzEx.Theming;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MediaPlayer.ApplicationSettings;
using MediaPlayer.Theming;
using MediaPlayer.ViewModel;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for ViewApplicationSettings.xaml
    /// </summary>
    public partial class ViewApplicationSettings : MetroWindow
    {
        public ViewApplicationSettings(ViewModelApplicationSettings vm)
        {
            InitializeComponent();

            DataContext = vm;
        }
    }
}
