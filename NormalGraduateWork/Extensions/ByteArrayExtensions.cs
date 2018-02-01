using System;
using System.Diagnostics;

namespace NormalGraduateWork.Extensions
{
    public static class ByteArrayExtensions
    {
        public static ushort ToUInt16(this byte[] array)
        {
            if (array.Length != 2)
                throw new ArgumentException(nameof(array));
            var uInt16 = (ushort) array[0];
            uInt16 <<= 8;
            return (ushort)(uInt16 | array[1]);
        }
        
    }
}