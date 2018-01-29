using System;
using System.Security.Cryptography;

// ReSharper disable BuiltInTypeReferenceStyle

namespace NormalGraduateWork.Random
{
    public class WrappedRandomNumberGenerator
    {
        private readonly RandomNumberGenerator randomNumberGenerator;

        public WrappedRandomNumberGenerator(RandomNumberGenerator randomNumberGenerator)
        {
            this.randomNumberGenerator = randomNumberGenerator;
        }

        public UInt32 GetNextUInt32()
        {
            var randomBytes = GetNextBytes(4);
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        public UInt64 GetNextUInt64()
        {
            var randomBytes = GetNextBytes(8);
            return BitConverter.ToUInt64(randomBytes, 0);
        }

        private byte[] GetNextBytes(int numberOfBytes)
        {
            var bytes = new byte[numberOfBytes];
            randomNumberGenerator.GetBytes(bytes);
            return bytes;
        }
    }
}