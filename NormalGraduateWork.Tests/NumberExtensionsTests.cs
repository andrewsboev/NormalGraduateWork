using System;
using FluentAssertions;
using NormalGraduateWork.Extensions;
using NUnit.Framework;

namespace NormalGraduateWork.Tests
{
    [TestFixture]
    public class NumberExtensionsTests
    {
        [Test]
        public void Test()
        {
            for (var i = 0; i <= 65535; ++i)
            {
                var asUshort = (ushort) i;
                var bytes = BitConverter.GetBytes(asUshort);
                Array.Reverse(bytes);
                var actualBytes = asUshort.GetBytes();
                actualBytes[0].Should().Be(bytes[0]);
                actualBytes[1].Should().Be(bytes[1]);
            }
        }
    }
}