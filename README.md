# Media-Player

This is a Media Player that I am actively working on. This is made for personal use only. I do not own any of the images used in this software. 

(Recently ported to .NET 6 - Startup assembly is 'MediaPlayer.Shell')

![Alt Text](https://i.imgur.com/Xze4BCG.gif)

![Screenshot](./Screenshots/Main.JPG?raw=true "Screenshot")

Features : 
- Themes (Thanks to the amazing Mahapps Metro UI library!)
- Opacity Slider
- Automatically download album art / lyrics of music that you're currently listening to. (Using caching and parallel threads for better responsiveness                   and performance)
- Setting available to save updated lyrics and album art to audio file's metadata
- Easy viewing of lyrics while listening
- MP3, FLAC, M4A and WMA Audio formats supported as of this moment. It uses Microsoft's 'Media Element' so it technically can play anything that Media Element can, but   I am only adding support incrementally for specific file formats that I can test and verify works.
- Drag & Drop support
- Shuffle functionality will physically re-order the list as opposed to choosing next track at random
- Hotkeys - Media keys on your keyboard will work if app has focus (Play/Pause, Next, Previous, Stop)
- Single Instance Support - More than one instance cannot be started and if subsequent instances are started and have startup arguments, they're sent to the first       instance via Named Pipes (This is important because it means you can set the media player as your default in windows, highlight x amount of audio files, press enter   and they will load in a single instance as opposed to opening x amount of media player instances in parallel, all containing a different file)

Features to come :

- Video playback support
- Anything else random that I can think of and feel like working on

Libaries used :

- Visual Studio 2022 Community Edition
- C# with MVVM Design Pattern
- Mahapps Metro 
- Mahapps Metro Icons
- FontAwesome5.WPF
- Flurl
- Flurl.Http
- LazyCache
- TaglibSharp
- Newtonsoft.Json

External API's used:
- LastFM
- Lyrics OVH

![Screenshot - Lyrics Collapsed](./Screenshots/LyricsCollapsed.JPG?raw=true "Screenshot - Lyrics Collapsed")

![Screenshot - Lyrics Expanded](./Screenshots/LyricsExpanded.JPG?raw=true "Screenshot - Lyrics Expanded")
