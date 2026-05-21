using MediaPlayer.ViewModel;
using System.ComponentModel.Composition;
using System.Windows;
using Wpf.Ui.Controls;

namespace MediaPlayer.View.Views
{
    [Export]
    public partial class ViewMediaPlayer : FluentWindow
    {
        [ImportingConstructor]
        public ViewMediaPlayer()
        {
            InitializeComponent();
        }

        [Import]
        public PlayerShellViewModel ViewModel
        {
            get => DataContext as PlayerShellViewModel;
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
