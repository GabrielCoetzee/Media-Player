namespace MediaPlayer.Theming.Abstract
{
    public interface IThemeManager
    {
        void ChangeTheme(string baseColor, string accent);

        void ChangeOpacity(double opacity);

    }
}
