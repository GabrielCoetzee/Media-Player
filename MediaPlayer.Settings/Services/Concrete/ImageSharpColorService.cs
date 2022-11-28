using MediaPlayer.Common.Constants;
using MediaPlayer.Settings.Services.Abstract;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace MediaPlayer.Settings.Services.Concrete
{
    [Export(ServiceNames.ImageSharpColorService, typeof(IColorService))]
    public class ImageSharpColorService : IColorService
    {
        public async Task<System.Windows.Media.Color> GetDominantColorAsync(byte[] imageBytes)
        {
            var color = new System.Windows.Media.Color();

            await Task.Run(() => 
            {
                using var image = Image.Load<Rgba32>(imageBytes);

                image.Mutate(x => x.Resize(new ResizeOptions { Sampler = KnownResamplers.NearestNeighbor, Size = new Size(100, 0) }));

                int r = 0;
                int g = 0;
                int b = 0;
                int totalPixels = 0;

                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        var pixel = image[x, y];

                        r += Convert.ToInt32(pixel.R);
                        g += Convert.ToInt32(pixel.G);
                        b += Convert.ToInt32(pixel.B);

                        totalPixels++;
                    }
                }

                r /= totalPixels;
                g /= totalPixels;
                b /= totalPixels;

                var dominantColor = new Rgba32((byte)r, (byte)g, (byte)b, 255);

                color =  new System.Windows.Media.Color()
                {
                    R = dominantColor.R,
                    G = dominantColor.G,
                    B = dominantColor.B,
                    A = dominantColor.A
                };
            });

            return color;
        }
    }
}
