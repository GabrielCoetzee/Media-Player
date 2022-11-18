using System.Threading.Tasks;

namespace MediaPlayer.Model.Metadata.Abstract.Updaters
{
    public interface ILyricsMetadataUpdater
    {
        Task<string> GetLyricsAsync(string artist, string track);
    }
}
