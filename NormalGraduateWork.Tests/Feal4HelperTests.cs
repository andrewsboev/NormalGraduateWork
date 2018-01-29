using FluentAssertions;
using NormalGraduateWork.Cryptography;
using NUnit.Framework;

namespace NormalGraduateWork.Tests
{
    using System;
    
    public class Feal4HelperTests
    {
        [Test]
        public void TestRotateLeft()
        {
            const byte value = (byte) (0b10111101);
            const byte expectedRotated = (byte) (0b11110110);
            var actualRotated = Feal4Helper.Rotl2(value);
            actualRotated.Should().Be(expectedRotated);
        }

        [Test]
        public void TestGetNthByte()
        {
            const UInt32 value = 0b11010101111111110110101000001000;
            const byte expectedZeroByte = 0b00001000;
            const byte expectedFirstByte = 0b01101010;
            const byte expectedSecondByte = 0b11111111;
            const byte expectedThirdByte = 0b11010101;

            Feal4Helper.GetNthByte(value, 0).Should().Be(expectedZeroByte);
            Feal4Helper.GetNthByte(value, 1).Should().Be(expectedFirstByte);
            Feal4Helper.GetNthByte(value, 2).Should().Be(expectedSecondByte);
            Feal4Helper.GetNthByte(value, 3).Should().Be(expectedThirdByte);
        }

        [Test]
        public void TestCombineBytes()
        {
            const byte zeroByte = 0b00001000;
            const byte firstByte = 0b01101010;
            const byte secondByte = 0b11111111;
            const byte thirdByte = 0b11010101;

            Feal4Helper.CombineBytes(thirdByte, secondByte, firstByte, zeroByte).Should()
                .Be(0b11010101111111110110101000001000);
        }

        [Test]
        public void TestGetLeftHalf()
        {
            const UInt64 value = 0b1101010111111111011010100000100000010000010101101111111110101011;
            Feal4Helper.GetLeftHalf(value).Should().Be(0b11010101111111110110101000001000);
        }

        [Test]
        public void TestGetRightHalf()
        {
            const UInt64 value = 0b1101010111111111011010100000100000010000010101101111111110101011;
            Feal4Helper.GetRightHalf(value).Should().Be(0b00010000010101101111111110101011);
        }

        [Test]
        public void TestCombine32BitHalfs()
        {
            const UInt32 leftHalf = 0b11010101111111110110101000001000;
            const UInt32 rightHalf = 0b00010000010101101111111110101011;
            const UInt64 expectedResult = 0b1101010111111111011010100000100000010000010101101111111110101011;
            Feal4Helper.Combine32BitHalfs(leftHalf, rightHalf).Should().Be(expectedResult);
        }
    }
}