using System.Drawing;

namespace MediaPlayer.Helpers.Extension_Methods
{
    public static class ExtensionMethods
    {
        public static byte[] ConvertPathToByteArray(this string filePath)
        {
            return (byte[])new ImageConverter().ConvertTo(Image.FromFile(filePath), typeof(byte[]));
        }
    }
}
