namespace Generic.Extensions
{
    public static class ByteExtensions
    {
        public static bool IsNullOrEmpty(this byte[] array)
        {
            return array == null || array.Length == 0 || array == default;
        }
    }
}
