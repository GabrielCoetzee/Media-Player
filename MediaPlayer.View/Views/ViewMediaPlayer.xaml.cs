using Generic.DependencyInjection;
using MahApps.Metro.Controls;
using MediaPlayer.ViewModel;
using System.ComponentModel.Composition;
using System.Windows;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class ViewMediaPlayer : MetroWindow
    {
        [ImportingConstructor]
        public ViewMediaPlayer()
        {
            InitializeComponent();

            AllowsTransparency = true;
        }

        [Import]
        public MainViewModel ViewModel
        {
            get => DataContext as MainViewModel;
            set => DataContext = value;
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
