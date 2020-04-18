using System.Windows;
using MahApps.Metro;

namespace MediaPlayer.ApplicationSettings.ThemeChanger
{
    public class ThemeChanger : IThemeChanger
    {
        public void ChangeOpacity(double opacity)
        {
            Application.Current.MainWindow.Background.Opacity = (double)opacity;
        }

        public void ChangeTheme(string selectedTheme)
        {
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(selectedTheme), ThemeManager.GetAppTheme("BaseDark"));
        }
    }
}
