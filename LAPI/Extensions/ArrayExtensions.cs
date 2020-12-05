namespace LAPI.Extensions
{
    public static class ArrayExtensions
    {
        public static bool StartsWith(this byte[] buffer, byte[] other)
        {
            if (buffer.Length < other.Length)
            {
                return false;
            }
            for (int i = 0; i < other.Length; i += 1)
            {
                if (buffer[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}