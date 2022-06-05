# Media-Player

This is a Media Player that I am actively working on. This is made for personal use only. I do not own any of the images used in this software, and this software will not be distributed. (Recently ported to .NET 6 - Startup assembly is 'MediaPlayer.Shell')

![Screenshot](./Screenshots/Main.JPG?raw=true "Screenshot")

Features : 
- Themes
- Transparency Slider
- Automatically download album art / lyrics of music that you're currently listening to. (Using caching and parallel threads for better responsiveness                   and performance)
- Automatically save updated lyrics and album art to audio file's metadata
- Easy viewing of lyrics while listening
- MP3, FLAC, M4A and WMA Audio formats supported as of this moment. It uses Microsoft MediaElement so it technically can play anything that MediaElement can, but I am   only adding support incrementally for specific file formats that I can test and verify works.
- Drag & Drop support
- Shuffle functionality will physically re-order the list as opposed to choosing next track at random
- Hotkeys - Media keys on your keyboard will work (Play/Pause, Next, Previous, Stop)
- Single Instance Support - More than one instance cannot be started and if subsequent instances are started and have startup arguments, they're sent to the first       instance via Named Pipes

Features to come :

- Installer
- Video playback support

Technologies used :

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

![Screenshot - Lyrics Collapsed](./Screenshots/LyricsCollapsed.JPG?raw=true "Screenshot - Lyrics Collapsed")

![Screenshot - Lyrics Expanded](./Screenshots/LyricsExpanded.JPG?raw=true "Screenshot - Lyrics Expanded")
