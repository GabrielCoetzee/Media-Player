using System.Windows;
using ControlzEx.Theming;
using MediaPlayer.Theming.Abstract;

namespace MediaPlayer.Theming.Concrete
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
