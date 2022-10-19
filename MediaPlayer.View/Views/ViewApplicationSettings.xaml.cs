using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using ControlzEx.Theming;
using Generic;
using Generic.DependencyInjection;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MediaPlayer.Theming;
using MediaPlayer.ViewModel;

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
        public ApplicationSettingsViewModel ViewModel
        {
            get => DataContext as ApplicationSettingsViewModel;
            set => DataContext = value;
        }
    }
}
