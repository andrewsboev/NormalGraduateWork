using System;

// ReSharper disable BuiltInTypeReferenceStyle

namespace NormalGraduateWork.Cryptography
{
    public static class Feal4Helper
    {
        public static UInt32 Fbox(UInt32 plain)
        {
            var x0 = GetNthByte(plain, 0);
            var x1 = GetNthByte(plain, 1);
            var x2 = GetNthByte(plain, 2);
            var x3 = GetNthByte(plain, 3);

            var t0 = (byte)(x2 ^ x3);

            var y1 = Gbox((byte)(x0 ^ x1), t0, 1);
            var y0 = Gbox(x0, y1, 0);
            var y2 = Gbox(t0, y1, 0);
            var y3 = Gbox(x3, y2, 1);

            return CombineBytes(y3, y2, y1, y0);
        }
        
        public static UInt32 CombineBytes(byte b3, byte b2, byte b1, byte b0)
        {
            return (UInt32) ((b3 << 24) | (b2 << 16) | (b1 << 8) | b0);
        }

        private static byte Gbox(byte a, byte b, byte mode)
        {
            unchecked
            {
                return Rotl2((byte)(a + b + mode));     
            }
        }
        
        public static byte Rotl2(byte value)
        {
            var firstShifted = (byte) (value << 2);
            var secondShifted = (byte) (value >> 6);
            return (byte)(firstShifted | secondShifted);
        }

        public static byte GetNthByte(UInt32 value, int n)
        {
            var shifted = value >> (8 * n);
            return (byte) (shifted & 0x000000FF);
        }
        
        public static UInt32 GetLeftHalf(UInt64 value)
        {
            var shifted = value >> 32;
            return (UInt32) shifted;
        }

        public static UInt32 GetRightHalf(UInt64 value)
        {
            var withZeroLeftHalf = value & (0b0000000000000000000000000000000011111111111111111111111111111111);
            return (UInt32) withZeroLeftHalf;
        }

        public static UInt64 Combine32BitHalfs(UInt32 left, UInt32 right)
        {
            var shifted = ((UInt64)left) << 32;
            return shifted | right;
        }
    }
}