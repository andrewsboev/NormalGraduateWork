using System;
using System.Collections;

namespace NormalGraduateWork.Extensions
{
    public static class BitArrayExtensions
    {
        public static BitArray NormalXor(this BitArray first, BitArray second)
        {
            var result = new BitArray(first.Count);
            for (var i = 0; i < first.Count; ++i)
                result[i] = first[i] != second[i];
            return result;
        }
        
        public static byte ToByte(this BitArray bitArray)
        {
            if (bitArray.Count > 8)
                throw new ArgumentException("BitArray length could not be more than 8");
            
            byte result = 0;
            for (var i = 0; i < bitArray.Length; ++i)
            {
                if (bitArray[i])
                    result |= 1;
                if (i != bitArray.Count - 1)
                    result <<= 1;
            }
            return result;
        }
        
        public static BitArray Merge(this BitArray first, BitArray second)
        {
            var result = new BitArray(first.Count + second.Count);
            for (var i = 0; i < first.Count; ++i)
                result[i] = first[i];
            for (var i = 0; i < second.Count; ++i)
                result[i + first.Count] = second[i];
            return result;
        }

        public static string ToNormalString(this BitArray bitArray)
        {
            var sb = string.Empty;
            for (var i = 0; i < bitArray.Count; ++i)
                sb += (bitArray[i] ? '1' : '0');
            return sb;
        }
    }
}