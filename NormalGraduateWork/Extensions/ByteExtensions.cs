using System.Collections;

namespace NormalGraduateWork.Extensions
{
    public static class ByteExtensions
    {
        public static BitArray ToBitArray(this byte value)
        {
            var result = new BitArray(8);
            for (var i = 0; i < 8; ++i)
            {
                result[7 - i] = (value & 1) == 1;
                value >>= 1;
            }

            return result;
        }
    }
}