using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace NormalGraduateWork.Cryptography.Aes16
{
    public class Aes16Encryptor
    {
        private readonly Aes16SubKeysGenerator aesSubKeysGenerator = new Aes16SubKeysGenerator();

        public byte[] Encrypt(byte[] plainText, byte[] key)
        {
            if (plainText.Length % 2 != 0)
                throw new ArgumentException("Plaintext length should be even number");
            
            var subKeys = aesSubKeysGenerator.GetAllSubKeys(key);
            return Encrypt(plainText, subKeys);
        }

        public byte[] Encrypt(byte[] plainText, IList<byte[]> subKeys)
        {
            var encryptionResult = new byte[plainText.Length];
            for (var i = 0; i < plainText.Length; i += 2)
            {
                var plainTextBytes = plainText.Skip(i).Take(2).ToArray();
                var zeroRoundResult = GetZeroRoundResult(plainTextBytes, subKeys[0]);
                var firstRoundResult = GetFirstRoundResult(zeroRoundResult, subKeys[1]);
                var secondRoundResult = GetSecondRoundResult(firstRoundResult, subKeys[2]);
                encryptionResult[i] = secondRoundResult[0];
                encryptionResult[i + 1] = secondRoundResult[1];
            }
            return encryptionResult;
        }

        private byte[] GetZeroRoundResult(byte[] plainText, byte[] roundKey)
        {
            return Aes16Helper.AddRoundKey(plainText, roundKey);
        }

        private byte[] GetFirstRoundResult(byte[] zeroRoundResult, byte[] roundKey)
        {
            // Nibble substitution
            var nibbled = Aes16Helper.NibbleSubstitution(zeroRoundResult);
            
            // Shift row
            var shiftedRowBytes = Aes16Helper.ShiftRow(nibbled);
            var mixedBytes = Aes16Helper.MixColumns(shiftedRowBytes);
            
            return Aes16Helper.AddRoundKey(new[] {mixedBytes[0], mixedBytes[1]}, roundKey);
        }

        private byte[] GetSecondRoundResult(byte[] firstRoundResult, byte[] subKey)
        {
            var nibbled = Aes16Helper.NibbleSubstitution(firstRoundResult);
            var secondFourBitsOfFirstByte = (byte) (nibbled[0] & 0b00001111);
            var secondFourBitsOfSecondByte = (byte) (nibbled[1] & 0b00001111);
            var newFirstByte = (byte) ((nibbled[0] & 0b11110000) | secondFourBitsOfSecondByte);
            var newSecondByte = (byte) ((nibbled[1] & 0b11110000) | secondFourBitsOfFirstByte);
            var shiftedBytes = new[] {newFirstByte, newSecondByte};
            return Aes16Helper.AddRoundKey(shiftedBytes, subKey);
        }
    }
}