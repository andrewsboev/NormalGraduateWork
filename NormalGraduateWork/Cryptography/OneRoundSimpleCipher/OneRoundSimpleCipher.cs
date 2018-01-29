using System.Collections.Generic;

namespace NormalGraduateWork.Cryptography.OneRoundSimpleCipher
{
    public class OneRoundSimpleCipher
    {
        private readonly Dictionary<int, int> sBox = new Dictionary<int, int>()
        {
            {0, 3},
            {1, 14},
            {2, 1},
            {3, 10},
            {4, 4},
            {5, 9},
            {6, 5},
            {7, 6},
            {8, 8},
            {9, 11},
            {10, 15},
            {11, 2},
            {12, 13},
            {13, 12},
            {14, 0},
            {15, 7}
        };
        
        private readonly Dictionary<int, int> inversedSBox = new Dictionary<int, int>()
        {
            {3, 0},
            {14, 1},
            {1, 2},
            {10, 3},
            {4, 4},
            {9, 5},
            {5, 6},
            {6, 7},
            {8, 8},
            {11, 9},
            {15, 10},
            {2, 11},
            {13, 12},
            {12, 13},
            {0, 14},
            {7, 15}
        };
        
        public byte[] Encrypt(byte[] plainText, byte key)
        {
            var encrypted = new byte[plainText.Length];
            for (var i = 0; i < plainText.Length; ++i)
            {
                var encryptedByte = EncryptByte(plainText[i], key);
                encrypted[i] = encryptedByte;
            }
            return encrypted;
        }

        public byte[] Decrypt(byte[] cipherText, byte key)
        {
            var decrypted = new byte[cipherText.Length];
            for (var i = 0; i < cipherText.Length; ++i)
            {
                var decryptedByte = DecryptByte(cipherText[i], key);
                decrypted[i] = decryptedByte;
            }
            return decrypted;
        }

        private byte DecryptByte(byte cipherByte, byte key)
        {
            var cipherFirstFourBits = (byte) (cipherByte >> 4);
            var cipherSecondFourBits = (byte) (cipherByte & 0b00001111);

            var decryptedFirstFourBits = DecryptLastFourBits(cipherFirstFourBits, key);
            var decryptedSecondFourBits = DecryptLastFourBits(cipherSecondFourBits, key);
            
            return (byte) ((decryptedFirstFourBits << 4) | decryptedSecondFourBits);
        }

        private byte DecryptLastFourBits(byte cipherByte, byte key)
        {
            var keyFirstFourBits = (byte)(key >> 4);
            var keySecondFourBits = (byte)(key & 0b00001111);

            var inversedSBoxed = (byte)inversedSBox[cipherByte];
            var inversedSecondKey = (byte) (inversedSBoxed ^ keySecondFourBits);
            var secondInversedSBoxed = (byte) inversedSBox[inversedSecondKey];
            var inversedFirstKey = (byte) (secondInversedSBoxed ^ keyFirstFourBits);
            
            return inversedFirstKey;
        }

        private byte EncryptByte(byte plainByte, byte key)
        {
            var plainFirstFourBits = (byte)(plainByte >> 4);
            var plainSecondFourBits = (byte)(plainByte & 0b00001111);

            var encryptedFirstFourBits = EncryptLastFourBits(plainFirstFourBits, key);
            var encryptedSecondFourBits = EncryptLastFourBits(plainSecondFourBits, key);

            return (byte) ((encryptedFirstFourBits << 4) | encryptedSecondFourBits);
        }

        private byte EncryptLastFourBits(byte plainByte, byte key)
        {
            var keyFirstFourBits = (byte)(key >> 4);
            var keySecondFourBits = (byte)(key & 0b00001111);

            var addedFirstKey = (byte) (plainByte ^ keyFirstFourBits);
            var sBoxed = (byte)sBox[addedFirstKey];
            var addedSecondKey = (byte) (sBoxed ^ keySecondFourBits);
            return addedSecondKey;
        }
    }
}