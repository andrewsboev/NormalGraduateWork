using System.Collections;
using System.Linq.Expressions;
using FluentAssertions;
using NormalGraduateWork.Cryptography.SDES;
using NormalGraduateWork.Extensions;
using NUnit.Framework;

namespace NormalGraduateWork.Tests
{
    [TestFixture]
    public class SimplifiedDesTests
    {
        [Test]
        public void Test()
        {
            var ba = new BitArray(5)
            {
                [0] = true,
                [1] = false,
                [2] = true,
                [3] = false,
                [4] = true
            };
            var shifted = SimplifiedDesHelper.LeftShift(ba, 1);
            shifted[0].Should().BeFalse();
            shifted[1].Should().BeTrue();
            shifted[2].Should().BeFalse();
            shifted[3].Should().BeTrue();
            shifted[4].Should().BeTrue();
        }
        
        [Test]
        public void Test1()
        {
           /* 
            
            for (var i = 0; i < 256; ++i)
            {
                var asByte = (byte) i;
                var encrypted = encryptor.Encrypt(asByte, key);
                var decrypted = encryptor.Decrypt(encrypted, key);
                decrypted.Should().Be(asByte);
            }*/
        }
        
        [Test]
        public void Test2()
        {
            var key = new BitArray(10)
            {
                [0] = true,
                [1] = false,
                [2] = false,
                [3] = false,
                [4] = true,
                [5] = true,
                [6] = false,
                [7] = false,
                [8] = true,
                [9] = false
            };
            var encryptor1 = new SDES("1000110010");
            var encryptor2 = new SimplifiedDes();
            for (var i = 0; i < 256; ++i)
            {
                var asByte = (byte) i;
                var encrypted1 = encryptor1.Encrypt(asByte);
                var encrypted2 = encryptor2.Encrypt(asByte, key);
                encrypted1.Should().Be(encrypted2);
                var decrypted1 = encryptor1.Decrypt(encrypted1);
                var decrypted2 = encryptor2.Decrypt(encrypted2, key);
                decrypted1.Should().Be(decrypted2);
                decrypted1.Should().Be(asByte);
            }
        }

        [Test]
        public void TempTest()
        {
            var first = new BitArray(3)
            {
                [0] = true,
                [1] = false,
                [2] = true
            };
            var second = new BitArray(3)
            {
                [0] = false,
                [1] = true,
                [2] = true
            };
            var merged = first.Merge(second);
            merged[0].Should().BeTrue();
            merged[1].Should().BeFalse();
            merged[2].Should().BeTrue();
            merged[3].Should().BeFalse();
            merged[4].Should().BeTrue();
            merged[5].Should().BeTrue();
        }
    }
}