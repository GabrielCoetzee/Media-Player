namespace MediaPlayer.Settings.Abstract
{
    public interface IThemeManager
    {
        string BackgroundColor { get; }
        string ForegroundColor { get; }
        string BaseColor { get; set; }
        string Accent { get; set; }
        decimal Opacity { get; set; }
        void ChangeTheme(string baseColor, string accent);
        void ChangeOpacity(double opacity);
        void SaveSettings();
    }
}
