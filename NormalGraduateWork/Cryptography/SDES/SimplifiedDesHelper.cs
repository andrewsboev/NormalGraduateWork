using System.Collections;

namespace NormalGraduateWork.Cryptography.SDES
{
    public static class SimplifiedDesHelper
    {
        public static BitArray GetLeftPart(BitArray bitArray)
        {
            var result = new BitArray(bitArray.Count / 2);
            for (var i = 0; i < bitArray.Count / 2; ++i)
                result[i] = bitArray[i];
            return result;
        }

        public static BitArray GetRightPart(BitArray bitArray)
        {
            var result = new BitArray(bitArray.Count / 2);
            for (var i = bitArray.Count / 2; i < bitArray.Count; ++i)
                result[i - (bitArray.Count / 2)] = bitArray[i];
            return result;
        }
        
        public static BitArray LeftShift(BitArray key, int shift)
        {
            var result = new BitArray(key.Count);
            for (var i = 0; i < key.Count; ++i)
                result[i] = key[(shift + i) % key.Count];
            return result;
        }

        public static BitArray GetBits(BitArray from, int startIndex, int numberOfBits)
        {
            var result = new BitArray(numberOfBits);
            for (var i = 0; i < numberOfBits; ++i)
                result[i] = from[startIndex + i];
            return result;
        }
 
        // ok
        public static BitArray P8(BitArray bitArray)
        {
            return new BitArray(8)
            {
                [0] = bitArray[5],
                [1] = bitArray[2],
                [2] = bitArray[6],
                [3] = bitArray[3],
                [4] = bitArray[7],
                [5] = bitArray[4],
                [6] = bitArray[9],
                [7] = bitArray[8]
            };
        }

        // ok
        public static BitArray P4(BitArray key)
        {
            return new BitArray(4)
            {
                [0] = key[1],
                [1] = key[3],
                [2] = key[2],
                [3] = key[0]
            };
        }
        
        // ok
        public static BitArray P10(BitArray key)
        {
            return new BitArray(10)
            {
                [0] = key[2],
                [1] = key[4],
                [2] = key[1],
                [3] = key[6],
                [4] = key[3],
                [5] = key[9],
                [6] = key[0],
                [7] = key[8],
                [8] = key[7],
                [9] = key[5]
            };
        }
    }
}