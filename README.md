# Media-Player

This is a Media Player that I am actively working on. This is made for personal and non-commercial use only. I do not own any of the images used in this software.

(Targets .NET 10 (Windows) - Startup assembly is 'MediaPlayer.Shell')

<img width="1372" height="772" alt="Adobe Express - Video - Dark Mode" src="https://github.com/user-attachments/assets/b4ce2db9-cdeb-4b52-aa86-42d2451e847f" />

Features :
- Modern Fluent/WinUI 3 styling via the WPF-UI library, with `FluentWindow`, Mica/Acrylic backdrops and dynamic accent
- Auto Adjust Accent feature which auto detects the dominant color in the currently playing track's cover art and adjusts the app accent to that color (example gif above)
- Automatically download album art / lyrics for the music you're currently listening to (using caching and parallel threads for better responsiveness and performance)
- Setting available to save updated lyrics and album art to the audio file's metadata
- Easy viewing of lyrics while listening
- Video playback support
- MP3, FLAC, M4A and WMA audio formats supported as of this moment. It uses Microsoft's `MediaElement`, so it can technically play anything `MediaElement` can, but support is added incrementally for specific file formats that I can test and verify.
- Drag & Drop support
- Shuffle functionality physically re-orders the queue rather than picking the next track at random; the currently-playing track stays put while everything else shuffles around it
- Drag-reorder in the queue, with single-click-to-play and X-on-hover remove (with auto-advance when removing the playing track)
- Hotkeys - Media keys on your keyboard work if the app has focus (Play/Pause, Next, Previous, Stop), plus `Space`, `Ctrl+O`, `M`, `Ctrl+L` (lyrics), `Ctrl+Q` (queue) and `Esc`
- Single Instance Support - More than one instance cannot be started; if subsequent instances are started with startup arguments, those are forwarded to the first instance via Named Pipes (this means you can set the media player as your default in Windows, highlight x audio files, press enter, and they all load in a single instance instead of opening x parallel instances)

<img width="1370" height="772" alt="Adobe Express - Video - Light Mode" src="https://github.com/user-attachments/assets/389c020c-e886-446e-910e-66a12efdf80c" />

Features to come :

- Anything else random that I can think of and feel like working on

Libraries used :

- C# with MVVM design pattern
- WPF-UI (Wpf.Ui)
- Microsoft.Xaml.Behaviors.Wpf
- SixLabors.ImageSharp (dominant-color extraction for auto-accent)
- Flurl
- Flurl.Http
- TaglibSharp
- Newtonsoft.Json

External API's used:
- LastFM
- Lyrics OVH

<img width="1368" height="775" alt="image" src="https://github.com/user-attachments/assets/d16adbc7-f6b6-421f-a9fe-36024885783d" />


<img width="1369" height="770" alt="image" src="https://github.com/user-attachments/assets/7cd01883-6ce3-49ec-96d9-544c261abcac" />
