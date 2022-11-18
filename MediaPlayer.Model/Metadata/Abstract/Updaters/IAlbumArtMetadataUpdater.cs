using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Abstract.Updaters
{
    public interface IAlbumArtMetadataUpdater
    {
        Task<byte[]> GetAlbumArtAsync(string artist, string track);
    }
}
