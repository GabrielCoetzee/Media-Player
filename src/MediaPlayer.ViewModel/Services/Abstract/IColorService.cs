using System.Threading.Tasks;
using System.Windows.Media;

namespace MediaPlayer.Settings.Services.Abstract
{
    public interface IColorService
    {
        Task<Color> GetDominantColorAsync(byte[] imageBytes);
    }
}
