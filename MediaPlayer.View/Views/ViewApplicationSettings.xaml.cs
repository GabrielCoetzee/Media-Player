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
        #region Bindable Properties

        readonly ISettingsProvider SettingsProvider;

        #endregion

        readonly IThemeSelector _themeSelector;

        public ViewApplicationSettings(ViewModelApplicationSettings vm, ISettingsProvider settingsProvider, IThemeSelector themeSelector)
        {
            InitializeComponent();

            DataContext = vm;

            SettingsProvider = settingsProvider;
            _themeSelector = themeSelector;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ColorSchemes.ToList().ForEach(accent => ComboBoxAccents.Items.Add(accent));
        }

        private void ButtonCloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsProvider.SaveSettings();

            this.Close();
        }

        private void MetroWindow_Activated(object sender, System.EventArgs e)
        {
            this.LoadTheme();
        }

        private void LoadTheme()
        {
            this._themeSelector.ChangeAccent(this.SettingsProvider.SelectedAccent);
        }
    }
}
