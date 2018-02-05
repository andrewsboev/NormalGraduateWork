using System.CodeDom;
using System.Collections;
using FluentAssertions;
using NormalGraduateWork.Extensions;
using NUnit.Framework;

namespace NormalGraduateWork.Tests
{
    [TestFixture]
    public class BitArrayExtensionsTests
    {
        [Test]
        public void Test()
        {
            var bitArray = new BitArray(8)
            {
                [0] = true,
                [1] = false,
                [2] = false,
                [3] = false,
                [4] = true,
                [5] = true,
                [6] = true,
                [7] = true
            };
            const byte expectedByte = 0b10001111;
            var actualByte = bitArray.ToByte();
            actualByte.Should().Be(expectedByte);
        }

        [Test]
        public void ShortTest()
        {
            var bitArray = new BitArray(2)
            {
                [0] = true,
                [1] = true
            };
            const byte expectedByte = 0b11;
            var actualByte = bitArray.ToByte();
            actualByte.Should().Be(expectedByte);
        }
        
    }
}