using System;
using System.Collections.Generic;

namespace NormalGraduateWork.Cryptography.Aes16
{
    public class Aes16Helper
    {
        private static readonly Dictionary<byte, byte> sBoxNibble = new Dictionary<byte, byte>
        {
            {0b00000000, 0b00001001},
            {0b00000001, 0b00000100},
            {0b00000010, 0b00001010},
            {0b00000011, 0b00001011},

            {0b00000100, 0b00001101},
            {0b00000101, 0b00000001},
            {0b00000110, 0b00001000},
            {0b00000111, 0b00000101},

            {0b00001000, 0b00000110},
            {0b00001001, 0b00000010},
            {0b00001010, 0b00000000},
            {0b00001011, 0b00000011},

            {0b00001100, 0b00001100},
            {0b00001101, 0b00001110},
            {0b00001110, 0b00001111},
            {0b00001111, 0b00000111}
        };
        
        private static readonly Dictionary<byte, byte> inversedSBoxNibble = new Dictionary<byte, byte>
        {
            {0b00001001, 0b00000000},
            {0b00000100, 0b00000001},
            {0b00001010, 0b00000010},
            {0b00001011, 0b00000011},

            {0b00001101, 0b00000100},
            {0b00000001, 0b00000101},
            {0b00001000, 0b00000110},
            {0b00000101, 0b00000111},

            {0b00000110, 0b00001000},
            {0b00000010, 0b00001001},
            {0b00000000, 0b00001010},
            {0b00000011, 0b00001011},

            {0b00001100, 0b00001100},
            {0b00001110, 0b00001101},
            {0b00001111, 0b00001110},
            {0b00000111, 0b00001111}
        };
        
        private static readonly Aes16MatrixMultiplier aes16MatrixMultiplier = new Aes16MatrixMultiplier();

        public static byte[] AddRoundKey(byte[] bytes, byte[] roundKey)
        {
            var firstByte = (byte) (bytes[0] ^ roundKey[0]);
            var secondByte = (byte) (bytes[1] ^ roundKey[1]);
            return new[] {firstByte, secondByte};
        }

        public static byte[] ShiftRow(byte[] bytes)
        {
            var secondFourBitsOfFirstByte = (byte) (bytes[0] & 0b00001111);
            var secondFourBitsOfSecondByte = (byte) (bytes[1] & 0b00001111);
            var newFirstByte = (byte) ((bytes[0] & 0b11110000) | secondFourBitsOfSecondByte);
            var newSecondByte = (byte) ((bytes[1] & 0b11110000) | secondFourBitsOfFirstByte);
            return new[] {newFirstByte, newSecondByte};
        }
        
        public static byte SubNib(byte b)
        {
            var firstHalfByte = (byte) ((0b11110000 & b) >> 4);
            var secondHalfByte = (byte) (0b00001111 & b);
            var sFirstHalfByte = sBoxNibble[firstHalfByte];
            var sSecondHalfByte = sBoxNibble[secondHalfByte];
            return (byte) ((sFirstHalfByte << 4) | sSecondHalfByte);
        }

        public static byte InvSubNib(byte b)
        {
            var firstHalfByte = (byte) ((0b11110000 & b) >> 4);
            var secondHalfByte = (byte) (0b00001111 & b);
            var sFirstHalfByte = inversedSBoxNibble[firstHalfByte];
            var sSecondHalfByte = inversedSBoxNibble[secondHalfByte];
            return (byte) ((sFirstHalfByte << 4) | sSecondHalfByte);
        }

        public static byte[] NibbleSubstitution(byte[] bytes)
        {
            var sFirstByte = SubNib(bytes[0]);
            var sSecondByte = SubNib(bytes[1]);
            return new[] {sFirstByte, sSecondByte};
        }

        public static byte[] InverseNibbleSubstitution(byte[] bytes)
        {
            var iFirstByte = InvSubNib(bytes[0]);
            var iSecondByte = InvSubNib(bytes[1]);
            return new[] {iFirstByte, iSecondByte};
        }

        public static byte[] MixColumns(byte[] bytes)
        {
            var firstArray = new[,]
            {
                {(byte)1, (byte)4},
                {(byte)4, (byte)1}
            };
            return InternalMixColumns(bytes, firstArray);
        }

        public static byte[] InverseMixColumns(byte[] bytes)
        {
            var firstArray = new[,]
            {
                {(byte) 9, (byte) 2},
                {(byte) 2, (byte) 9}
            };
            return InternalMixColumns(bytes, firstArray);
        }

        private static byte[] InternalMixColumns(byte[] bytes, byte[,] matrix)
        {
            var newSecondFourBitsOfFirstByte = (byte) (bytes[0] & 0b00001111);
            var newSecondFourBitsOfSecondByte = (byte) (bytes[1] & 0b00001111);
            
            var secondArray = new[,]
            {
                {(byte)((bytes[0] & 0b11110000) >> 4), (byte)((bytes[1] & 0b11110000) >> 4)},
                {newSecondFourBitsOfFirstByte, newSecondFourBitsOfSecondByte}
            };
            var mixResult = aes16MatrixMultiplier.Multiply(matrix, secondArray);
            var firstByte = (byte)((mixResult[0, 0] << 4) | (mixResult[1, 0] & 0b00001111));
            var secondByte = (byte)((mixResult[0, 1] << 4) | (mixResult[1, 1] & 0b00001111));
            return new[] {firstByte, secondByte};
        }

        public static byte RotNib(byte b)
        {
            var firstFourBits = b & 0b11110000;
            var secondFourBits = b & 0b00001111;
            var shiftedFirstBits = (byte) (firstFourBits >> 4);
            var shiftedSecondBits = (byte) (secondFourBits << 4);
            return (byte) (shiftedSecondBits | shiftedFirstBits);
        }
        
        public static byte[,] GetInverseMatrix(byte[,] matrix)
        {
            for (var a = 0; a < 16; ++a)
            {
                for (var b = 0; b < 16; ++b)
                {
                    for (var c = 0; c < 16; ++c)
                    {
                        for (var d = 0; d < 16; ++d)
                        {
                            var arr = new[,]
                            {
                                {(byte)a, (byte)b},
                                {(byte)c, (byte)d}
                            };
                            var res = new Aes16MatrixMultiplier().Multiply(arr, matrix);
                            if (res[0, 0] == 1 && res[0, 1] == 0 && res[1, 0] == 0 && res[1, 1] == 1)
                                return arr;
                        }
                    }
                }
            }
            throw new ArgumentException();
        }
    }
}