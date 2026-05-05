using Generic.DependencyInjection;
using MediaPlayer.ViewModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
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

            MEF.Container?.SatisfyImportsOnce(this);
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

        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Shell.IsLyricsOpen = !Shell.IsLyricsOpen;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Q && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Shell.IsQueueOpen = !Shell.IsQueueOpen;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape && Shell.IsLyricsOpen)
            {
                Shell.IsLyricsOpen = false;
                e.Handled = true;
            }
        }
    }
}
