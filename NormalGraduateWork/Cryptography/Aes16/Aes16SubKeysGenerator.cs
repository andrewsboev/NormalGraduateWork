using System.Collections.Generic;

namespace NormalGraduateWork.Cryptography.Aes16
{
    public class Aes16SubKeysGenerator
    {
        public IList<byte[]> GetAllSubKeys(byte[] key)
        {
            var firstSubKey = GetFirstSubKey(key);
            var secondSubKey = GetSecondSubKey(key);
            var thirdSubKey = GetThirdSubKey(key);
            return new List<byte[]>
            {
                firstSubKey,
                secondSubKey,
                thirdSubKey
            };
        }

        private byte[] GetThirdSubKey(byte[] key)
        {
            var secondSubKey = GetSecondSubKey(key);
            var firstByte = (byte) (secondSubKey[0] ^ 0b00110000 ^ Aes16Helper.SubNib(Aes16Helper.RotNib(secondSubKey[1])));
            var secondByte = (byte) (firstByte ^ secondSubKey[1]);
            return new[] {firstByte, secondByte};
        }

        private byte[] GetSecondSubKey(byte[] key)
        {
            var firstSubKey = GetFirstSubKey(key);
            var a = (byte) (firstSubKey[0] ^ 0b10000000);
            var c = Aes16Helper.RotNib(firstSubKey[1]);
            var b = Aes16Helper.SubNib(c);
            var firstByte = (byte) (a ^ b);
            var secondByte = (byte) (firstByte ^ firstSubKey[1]);
            return new[] {firstByte, secondByte};
        }
        
        private byte[] GetFirstSubKey(byte[] key)
        {
            return key;
        }
    }
}