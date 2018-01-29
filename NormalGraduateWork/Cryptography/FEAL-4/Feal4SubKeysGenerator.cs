using System;
using System.Security.Cryptography;

namespace NormalGraduateWork.Cryptography
{
    public class Feal4SubKeysGenerator
    {
        public UInt32[] Generate()
        {
            var random = new RNGCryptoServiceProvider();
            var subKeys = new UInt32[6];
            for (var i = 0; i < subKeys.Length; ++i)
            {
                var bytes = new byte[4];
                random.GetBytes(bytes);
                var uInt32Value = BitConverter.ToUInt32(bytes, 0);
                subKeys[i] = uInt32Value;
            }
            return subKeys;
        }
    }
}