using System.ComponentModel.Composition;
using Generic.DependencyInjection;
using MahApps.Metro.Controls;
using MediaPlayer.Common.Constants;
using MediaPlayer.Settings.ViewModels;
using MediaPlayer.View.Services.Abstract;

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

            MEF.Container?.SatisfyImportsOnce(this);

            SetWindowResolution();
        }

        [Import]
        public SettingsViewModel ViewModel
        {
            get => DataContext as SettingsViewModel;
            set => DataContext = value;
        }

        [Import(ServiceNames.HardCodedWindowResolutionCalculator)]
        public IWindowResolutionCalculator WindowResolutionCalculator { get; set; }

        private void SetWindowResolution()
        {
            var resolution = WindowResolutionCalculator.CalculateOptimalSettingsWindowResolution();

            MinWidth = resolution.Width;
            MinHeight = resolution.Height;
        }
    }
}
