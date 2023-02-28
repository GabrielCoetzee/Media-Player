using Generic.DependencyInjection;
using MahApps.Metro.Controls;
using MediaPlayer.Common.Constants;
using MediaPlayer.View.Services.Abstract;
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

            MEF.Container?.SatisfyImportsOnce(this);

            SetWindowResolution();
        }

        private void SetWindowResolution()
        {
            var resolution = WindowResolutionCalculator.CalculateOptimalMainWindowResolution();

            MinWidth = resolution.Width;
            MinHeight = resolution.Height;
        }

        [Import]
        public MainViewModel ViewModel
        {
            get => DataContext as MainViewModel;
            set => DataContext = value;
        }

        [Import(ServiceNames.HardCodedWindowResolutionCalculator)]
        public IWindowResolutionCalculator WindowResolutionCalculator { get; set; }

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
