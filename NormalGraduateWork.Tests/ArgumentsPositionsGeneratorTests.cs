using System.Security.Cryptography;
using NormalGraduateWork.Random;
using NormalGraduateWork.TemplateGenerating;
using NUnit.Framework;

namespace NormalGraduateWork.Tests
{
    [TestFixture]
    public class ArgumentsPositionsGeneratorTests
    {
        [Test]
        public void Test()
        {
            var generator = new ArgumentsPositionsGenerator(
                new WrappedRandomNumberGenerator(new RNGCryptoServiceProvider()));

            var result = generator.Generate(3, 2);
        }
    }
}