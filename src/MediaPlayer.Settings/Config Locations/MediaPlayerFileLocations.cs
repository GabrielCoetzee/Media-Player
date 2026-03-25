using Generic.Settings.Abstract;
using Generic.Settings.Concrete;
using System.ComponentModel.Composition;

namespace MediaPlayer.Settings.ConfigLocations
{
    [InheritedExport(typeof(IFileLocations))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MediaPlayerFileLocations : FileLocations
    {
        public override string ApplicationName => "Media Player";
    }
}
