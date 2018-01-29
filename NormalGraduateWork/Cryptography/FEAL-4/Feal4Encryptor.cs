using System;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable ReturnTypeCanBeEnumerable.Local

namespace NormalGraduateWork.Cryptography
{
    public class Feal4Encryptor
    {
        public UInt64 Decrypt(UInt64 cipherText, UInt32[] subKeys)
        {
            var cipherSplittedIntoHalfs = Split64BitToHalfs(cipherText);
            var cipherLeft = cipherSplittedIntoHalfs[0];
            var cipherRight = cipherSplittedIntoHalfs[1];

            var round4Right = cipherLeft ^ cipherRight;
            var round4Left = cipherLeft ^ Feal4Helper.Fbox(round4Right ^ subKeys[3]);
            
            var round3Right = round4Left;
            var round3Left = round4Right ^ Feal4Helper.Fbox(round3Right ^ subKeys[2]);
            
            var round2Right = round3Left;
            var round2Left = round3Right ^ Feal4Helper.Fbox(round2Right ^ subKeys[1]);
            
            var leftHalf = round2Right ^ Feal4Helper.Fbox(round2Left ^ subKeys[0]);
            var rightHalf = round2Left ^ leftHalf;
            
            leftHalf ^= subKeys[4];
            rightHalf ^= subKeys[5];

            return (leftHalf << 32) | rightHalf;
        }
        
        public UInt64 Encrypt(UInt64 plainText, UInt32[] subKeys)
        {
            UInt32 leftHalf = Feal4Helper.GetLeftHalf(plainText);
            UInt32 rightHalf = Feal4Helper.GetRightHalf(plainText);

            leftHalf ^= subKeys[4];
            rightHalf ^= subKeys[5];
            
            var round2Left = leftHalf ^ rightHalf;
            var round2Right = leftHalf ^ Feal4Helper.Fbox(round2Left ^ subKeys[0]);
            
            var round3Left = round2Right;
            var round3Right = round2Left ^ Feal4Helper.Fbox(round2Right ^ subKeys[1]);

            var round4Left = round3Right;
            var round4Right = round3Left ^ Feal4Helper.Fbox(round3Right ^ subKeys[2]);
            
            var cipherLeft = round4Left ^ Feal4Helper.Fbox(round4Right ^ subKeys[3]);
            var cipherRight = cipherLeft ^ round4Right;

            return Feal4Helper.Combine32BitHalfs(cipherLeft, cipherRight);
        }

        private static UInt32[] Split64BitToHalfs(UInt64 full)
        {
            var right = (UInt32)(full & 0x00000000FFFFFFFF);
            var left = (UInt32) (full >> 32);
            return new[] {left, right};
        }
    }
}