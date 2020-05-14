using System.Windows;
using System.Windows.Input;
using ControlzEx.Theming;
using MahApps.Metro;

namespace MediaPlayer.Theming
{
    public class ThemeSelector : IThemeSelector
    {
        public void ChangeOpacity(double opacity)
        {
            Application.Current.MainWindow.Background.Opacity = opacity;
        }

        public void ChangeAccent(string accent)
        {
            foreach (Window window in Application.Current.Windows)
            {
                ThemeManager.Current.ChangeThemeColorScheme(window, accent);
            }
        }
    }
}
