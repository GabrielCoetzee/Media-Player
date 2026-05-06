using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Media;
using Generic.Extensions;
using Generic.Mediator;
using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Settings.Config;
using MediaPlayer.Settings.Services.Abstract;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace MediaPlayer.Settings.ViewModels
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

            var dominantColor = await _colorService.GetDominantColorAsync(albumArt);
            ApplicationAccentColorManager.Apply(dominantColor, CurrentTheme);
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
                OnPropertyChanged(nameof(EffectiveBackgroundColor));

                _themeSettings.Save();
                ApplicationThemeManager.Apply(CurrentTheme, CurrentBackdrop);
                Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
            }
        }

        public DwmBackdropType BackdropType
        {
            get => _themeSettings.BackdropType;
            set
            {
                _themeSettings.BackdropType = value;
                OnPropertyChanged(nameof(BackdropType));
                OnPropertyChanged(nameof(EffectiveBackgroundColor));

                _themeSettings.Save();
                ApplicationThemeManager.Apply(CurrentTheme, CurrentBackdrop);
                Messenger<MessengerMessages>.Send(MessengerMessages.AutoAdjustAccent);
            }
        }

        public Color EffectiveBackgroundColor =>
            BackdropType != DwmBackdropType.None
                ? Colors.Transparent
                : UseDarkMode ? Colors.Black : Colors.White;

        public bool IsBackdropSupported =>
            Environment.OSVersion.Version is { Major: >= 10, Build: >= 22621 };

        public void ResetThemeToDefaultSettings() =>
            ApplicationThemeManager.Apply(CurrentTheme, CurrentBackdrop);

        private ApplicationTheme CurrentTheme =>
            UseDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light;

        private WindowBackdropType CurrentBackdrop => BackdropType switch
        {
            DwmBackdropType.Auto    => WindowBackdropType.Auto,
            DwmBackdropType.None    => WindowBackdropType.None,
            DwmBackdropType.Mica    => WindowBackdropType.Mica,
            DwmBackdropType.Acrylic => WindowBackdropType.Acrylic,
            DwmBackdropType.Tabbed  => WindowBackdropType.Tabbed,
            _                       => WindowBackdropType.Auto
        };
    }
}
