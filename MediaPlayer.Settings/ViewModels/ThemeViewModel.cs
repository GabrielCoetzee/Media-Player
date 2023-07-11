using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using ControlzEx.Theming;
using Generic.Extensions;
using Generic.Mediator;
using Generic.PropertyNotify;
using MediaPlayer.Common.Constants;
using MediaPlayer.Common.Enumerations;
using MediaPlayer.Settings.Config;
using MediaPlayer.Settings.Services.Abstract;

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

            var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(BaseColor, dominantColor);

            ThemeManager.Current.AddTheme(theme);
            ThemeManager.Current.ChangeTheme(Application.Current, theme);
        }

        public string AccentLabel => !AutoAdjustAccent ? "Accent: " : "Default Accent: ";
        public string BaseColor => _themeSettings.BaseColor;
        public string BackgroundColor => UseDarkMode ? System.Drawing.Color.Black.Name.ToString() : System.Drawing.Color.White.Name.ToString();
        public string ForegroundColor => UseDarkMode ? System.Drawing.Color.White.Name.ToString() : System.Drawing.Color.Black.Name.ToString();

        public bool AutoAdjustAccent
        {
            get => _themeSettings.AutoAdjustAccent;
            set
            {
                _themeSettings.AutoAdjustAccent = value;
                OnPropertyChanged(nameof(AutoAdjustAccent));
                OnPropertyChanged(nameof(AccentLabel));

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
                OnPropertyChanged(nameof(BaseColor));
                OnPropertyChanged(nameof(BackgroundColor));
                OnPropertyChanged(nameof(ForegroundColor));

                ChangeBaseColor();
            }
        }

        public string Accent
        {
            get => _themeSettings.Accent;
            set
            {
                _themeSettings.Accent = value;
                OnPropertyChanged(nameof(Accent));

                ChangeAccent();
            }
        }

        public decimal Opacity
        {
            get => _themeSettings.Opacity;
            set
            {
                _themeSettings.Opacity = value;
                OnPropertyChanged(nameof(Opacity));

                ChangeOpacity();
            }
        }

        public void ResetThemeToDefaultSettings() => ThemeManager.Current.ChangeTheme(Application.Current, BaseColor, Accent);
        public void ChangeAccent() => ThemeManager.Current.ChangeThemeColorScheme(Application.Current, Accent);
        public void ChangeBaseColor() => ThemeManager.Current.ChangeThemeBaseColor(Application.Current, BaseColor);
        public void ChangeOpacity() => Application.Current.MainWindow.Background.Opacity = (double)Opacity;

        public void SaveSettings()
        {
            _themeSettings.Save();
        }
    }
}
