using System;
using FluentAssertions;
using NormalGraduateWork.Extensions;
using NUnit.Framework;

namespace NormalGraduateWork.Tests
{
    [TestFixture]
    public class ByteArrayExtensionsTests
    {
        [Test]
        public void Test()
        {
            for (var i = 0; i <= 65535; ++i)
            {
                var bytes = BitConverter.GetBytes((ushort)i);
                Array.Reverse(bytes);
                var asUInt16 = bytes.ToUInt16();
                asUInt16.Should().Be((ushort) i);
            }
        }
        
    }
}