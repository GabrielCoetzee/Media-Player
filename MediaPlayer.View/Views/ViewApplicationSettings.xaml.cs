using System.ComponentModel.Composition;
using MahApps.Metro.Controls;
using MediaPlayer.Settings.ViewModels;

namespace MediaPlayer.View.Views
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Interaction logic for ViewApplicationSettings.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ViewApplicationSettings : MetroWindow
    {
        [ImportingConstructor]
        public ViewApplicationSettings()
        {
            InitializeComponent();
        }

        [Import]
        public SettingsViewModel ViewModel
        {
            get => DataContext as SettingsViewModel;
            set => DataContext = value;
        }
    }
}
