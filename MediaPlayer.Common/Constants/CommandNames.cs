namespace MediaPlayer.Common.Constants
{
    public static class CommandNames
    {
        public const string LoadThemeOnWindowLoaded = "Load Theme";
        public const string SaveSettings = "Save Settings";
        public const string Shuffle = "Shuffle";
        public const string OpenSettingsWindow = "Open Settings Window";
        public const string PlayPause = "Play / Pause";
        public const string Mute = "Mute";
        public const string PreviousTrack = "Previous Track";
        public const string NextTrack = "Next Track";
        public const string Stop = "Stop";
        public const string Repeat = "Repeat";
        public const string ClearList = "Clear List";
        public const string StartedDragging = "StartedDragging"; //MEF doesn't accept pascal case here??
        public const string CompletedDragging = "Completed Dragging";
        public const string TopMostGridDragEnter = "Top Most Grid Drag Enter";
        public const string TopMostGridDrop = "Top Most Grid Drop";
        public const string MainWindowClosing = "Main Window Closing";
        public const string MediaOpened = "Media Opened";
        public const string AddMedia = "Add Media";
    }
}
