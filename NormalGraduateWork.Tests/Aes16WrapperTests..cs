using System.Text;
using FluentAssertions;
using NormalGraduateWork.Cryptography.Aes16;
using NUnit.Framework;

namespace NormalGraduateWork.Tests
{
    [TestFixture]
    public class Aes16WrapperTests
    {
        [Test]
        public void Test()
        {
            var key = new byte[]
            {
                0b10100111,
                0b00111011
            };
            var plainText = new byte[]
            {
                0b01101111,
                0b01101011
            };
            var expectedCipherText = new byte[]
            {
                0b00000111,
                0b00111000
            };
            var cipherText = new Aes16Wrapper().Encrypt(plainText, key);
            cipherText[0].Should().Be(expectedCipherText[0]);
            cipherText[1].Should().Be(expectedCipherText[1]);
        }
        
    }
}