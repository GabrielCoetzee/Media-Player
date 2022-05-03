using MahApps.Metro.Controls;
using MediaPlayer.ViewModel;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ViewMediaPlayer : MetroWindow
    {
        public ViewMediaPlayer(ViewModelMediaPlayer vm)
        {
            InitializeComponent();

            DataContext = vm;

            AllowsTransparency = true;
        }
    }
}
