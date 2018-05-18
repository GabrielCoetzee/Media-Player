namespace MediaPlayer.Interfaces
{
    public interface IExposeApplicationSettings
    {
        string[] SupportedAudioFormats { get; }

        string SelectedTheme { get; }

        decimal Opacity { get; }
    }
}
