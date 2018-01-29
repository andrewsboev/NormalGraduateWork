using System;
using System.Linq;

namespace NormalGraduateWork.Cryptography.Aes16
{
    public class Aes16Decryptor
    {
        private readonly Aes16SubKeysGenerator aesSubKeysGenerator = new Aes16SubKeysGenerator();
        
        public byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            if (cipherText.Length % 2 != 0)
                throw new ArgumentException("Ciphertext length should be even number");
            
            var allSubKeys = aesSubKeysGenerator.GetAllSubKeys(key);

            var decryptionResult = new byte[cipherText.Length];
            for (var i = 0; i < cipherText.Length; i += 2)
            {
                var cipherTextBytes = cipherText.Skip(i).Take(2).ToArray();
                var firstRoundCipherText = InverseSecondRound(cipherTextBytes, allSubKeys[2]);
                var zeroRoundCipherText = InverseFirstRound(firstRoundCipherText, allSubKeys[1]);
                var plainText = InverseZeroRound(zeroRoundCipherText, allSubKeys[0]);
                decryptionResult[i] = plainText[0];
                decryptionResult[i + 1] = plainText[1];
            }
            return decryptionResult;
        }

        private byte[] InverseSecondRound(byte[] cipherText, byte[] roundKey)
        {
            var inversedRoundKey = Aes16Helper.AddRoundKey(cipherText, roundKey);
            var inversedShiftRow = Aes16Helper.ShiftRow(inversedRoundKey);
            var inversedNibbleSubstitution = Aes16Helper.InverseNibbleSubstitution(inversedShiftRow);
            return inversedNibbleSubstitution;
        }

        private byte[] InverseFirstRound(byte[] firstRoundCipherText, byte[] roundKey)
        {
            var inversedRoundKey = Aes16Helper.AddRoundKey(firstRoundCipherText, roundKey);
            var inversedMixColumns = Aes16Helper.InverseMixColumns(inversedRoundKey);
            var inversedShiftRow = Aes16Helper.ShiftRow(inversedMixColumns);
            var inversedNibbleSubstitution = Aes16Helper.InverseNibbleSubstitution(inversedShiftRow);
            return inversedNibbleSubstitution;
        }

        private byte[] InverseZeroRound(byte[] zeroRoundCipherText, byte[] roundKey)
        {
            var inversedRoundKey = Aes16Helper.AddRoundKey(zeroRoundCipherText, roundKey);
            return inversedRoundKey;
        }
    }
}