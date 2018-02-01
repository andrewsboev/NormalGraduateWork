namespace NormalGraduateWork.Extensions
{
    public static class NumberExtensions
    {
        public static byte[] GetBytes(this ushort val)
        {
            var lastByte = (byte) (val & byte.MaxValue);
            var firstByte = (byte) (val >> 8);
            return new[] {firstByte, lastByte};
        }
    }
}