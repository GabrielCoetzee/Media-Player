# Media-Player

Features Recently added : 
- Themes! - Thanks to the amazing Mahapps Metro library :-) 
- Enable / Disable Transparency and slider to select preferred opacity
- Recently ported to .NET 6

This is a Media Player that I am actively working on. This is made for personal use only. I do not own any of the images used in this software, and this software will not be distributed.

Features Include : 

- MP3, FLAC, M4A and WMA Audio formats supported as of this moment. It uses Microsoft MediaElement so it technically can play anything that MediaElement can, but I am only adding supported incrementally for specific file formats that I can test and verify works.
- You can drag & drop files anywhere in the application. Supported files dropped in, if any found, will be added.
- Shuffle functionality will re-order the list with currently playing media item moved to the top
- Hotkeys - Media keys on your keyboard will work (Play/Pause, Next, Previoous, Stop)
- Lyrics found for the media item (Within the .mp3's metadata) will show in an expander on the top-left as shown in a below screenshots.

Features to come :

- Installer
- Have ability to set media player as default in windows
- Video playback
- Download and save album art and lyrics for media items while playing and if not found

Technologies used :

- Visual Studio 2022 Community Edition
- C# with MVVM Design Pattern
- Mahapps Metro 
- Mahapps Metro Icons
- FontAwesome.WPF

![Screenshot](./Screenshots/Main.JPG?raw=true "Screenshot")

![Screenshot - Lyrics Collapsed](./Screenshots/LyricsCollapsed.JPG?raw=true "Screenshot - Lyrics Collapsed")

![Screenshot - Lyrics Expanded](./Screenshots/LyricsExpanded.JPG?raw=true "Screenshot - Lyrics Expanded")
