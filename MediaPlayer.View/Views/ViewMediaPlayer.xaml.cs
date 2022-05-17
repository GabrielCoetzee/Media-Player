using Generic.Wrappers;
using MahApps.Metro.Controls;
using MediaPlayer.ViewModel;
using System.Windows;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ViewMediaPlayer : MetroWindow
    {
        public ViewMediaPlayer(MainViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;

            AllowsTransparency = true;
        }

        public void BringToForeground()
        {
            if (WindowState == WindowState.Minimized || Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }
    }
}
