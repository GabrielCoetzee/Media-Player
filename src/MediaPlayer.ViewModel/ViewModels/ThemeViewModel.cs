using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Generic.Extensions;
using Generic.Mediator;
using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Settings.Config;
using MediaPlayer.Settings.Services.Abstract;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace MediaPlayer.ViewModel.ViewModels
{
    [Export]
    public class ThemeViewModel : NotifyPropertyChanged
    {
        readonly ThemeSettings _themeSettings;
        readonly IColorService _colorService;

        [ImportingConstructor]
        public ThemeViewModel(ThemeSettings themeSettings,
            [Import(ServiceNames.ImageSharpColorService)] IColorService colorService)
        {
            _themeSettings = themeSettings;
            _colorService = colorService;
        }

        public async Task AutoAdjustAccentAsync(byte[] albumArt)
        {
            if (!AutoAdjustAccent || albumArt.IsNullOrEmpty())
            {
                ResetThemeToDefaultSettings();
                return;
            }

            try
            {
                var dominantColor = await _colorService.GetDominantColorAsync(albumArt);

                ApplicationAccentColorManager.Apply(dominantColor, CurrentTheme);
            }
            catch (Exception)
            {
                // Corrupt / undecodable album art — treat it like missing art rather than crashing.
                ResetThemeToDefaultSettings();
            }
        }

        public bool AutoAdjustAccent
        {
            get => _themeSettings.AutoAdjustAccent;
            set
            {
                _themeSettings.AutoAdjustAccent = value;
                OnPropertyChanged(nameof(AutoAdjustAccent));

                _themeSettings.Save();
                Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
            }
        }

        public bool UseDarkMode
        {
            get => _themeSettings.UseDarkMode;
            set
            {
                _themeSettings.UseDarkMode = value;
                OnPropertyChanged(nameof(UseDarkMode));

                _themeSettings.Save();
                ApplicationThemeManager.Apply(CurrentTheme, BackdropType);
                Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
            }
        }

        public WindowBackdropType BackdropType
        {
            get => _themeSettings.BackdropType;
            set
            {
                _themeSettings.BackdropType = value;
                OnPropertyChanged(nameof(BackdropType));

                _themeSettings.Save();
                ApplicationThemeManager.Apply(CurrentTheme, BackdropType);
                Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
            }
        }

        public bool IsBackdropSupported => Environment.OSVersion.Version is { Major: >= 10, Build: >= 22621 };

        private void ResetThemeToDefaultSettings() => ApplicationThemeManager.Apply(CurrentTheme, BackdropType);

        private ApplicationTheme CurrentTheme => UseDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light;
    }
}
