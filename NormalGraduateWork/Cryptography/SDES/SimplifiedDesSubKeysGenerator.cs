using System;
using System.Collections;
using NormalGraduateWork.Extensions;

namespace NormalGraduateWork.Cryptography.SDES
{
    public class SimplifiedDesSubKeysGenerator
    {
        public BitArray[] Generate(BitArray key)
        {
            if (key.Count != 10)
                throw new ArgumentException("Key length should be 10 bits");

            var p10 = SimplifiedDesHelper.P10(key);
            
            var first5Bits = SimplifiedDesHelper.GetBits(p10, 0, 5);
            var second5Bits = SimplifiedDesHelper.GetBits(p10, 5, 5);
            

            var first5BitsShifted = SimplifiedDesHelper.LeftShift(first5Bits, 1);
            var second5BitsShifted = SimplifiedDesHelper.LeftShift(second5Bits, 1);
            

            var mergedKey = first5BitsShifted.Merge(second5BitsShifted);
            var firstSubKey = SimplifiedDesHelper.P8(mergedKey);

            
            var a = SimplifiedDesHelper.LeftShift(first5Bits, 3);
            var b = SimplifiedDesHelper.LeftShift(second5Bits, 3);
            

            var mergedKeyAgain = a.Merge(b);
            var secondSubKey = SimplifiedDesHelper.P8(mergedKeyAgain);

            
            return new[] {firstSubKey, secondSubKey};
        }
    }
}