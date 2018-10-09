namespace MediaPlayer.Interfaces
{
    public interface IExposeApplicationSettings
    {
        string[] SupportedFormats { get; }

        string SelectedTheme { get; }

        decimal Opacity { get; }

        void SaveSettings();
    }
}
