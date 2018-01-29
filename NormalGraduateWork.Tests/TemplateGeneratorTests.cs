using System.Security.Cryptography;
using NormalGraduateWork.Extensions;
using NormalGraduateWork.Random;
using NormalGraduateWork.TemplateGenerating;
using NUnit.Framework;

namespace NormalGraduateWork.Tests
{
    [TestFixture]
    public class TemplateGeneratorTests
    {
        private TemplateBuilder templateBuilder;

        [SetUp]
        public void SetUp()
        {
            var randomNumberGenerator = new RNGCryptoServiceProvider();
            var wrappedRandomGenerator = new WrappedRandomNumberGenerator(randomNumberGenerator);
            var stringGenerator = new StringGenerator(wrappedRandomGenerator);
            templateBuilder = new TemplateBuilder(stringGenerator);
        }
        
        [Test]
        public void Test1()
        {
            DoAssert(1, new[] {0}, "{0}a");
        }

        [Test]
        public void Test2()
        {
            DoAssert(1, new[] {1}, "a{0}");   
        }

        [Test]
        public void Test3()
        {
            DoAssert(1, new[] {0, 1}, "{0}a{1}");
        }

        [Test]
        public void Test4()
        {
            DoAssert(2, new[] {0}, "{0}aa");
        }

        [Test]
        public void Test5()
        {
            DoAssert(2, new[] {1}, "a{0}a");
        }

        [Test]
        public void Test6()
        {
            DoAssert(2, new[] {2}, "aa{0}");
        }

        [Test]
        public void Test7()
        {
            DoAssert(2, new[] {0, 2}, "{0}aa{1}");
            DoAssert(2, new[] {0, 1}, "{0}a{1}a");
        }

        private void DoAssert(int templateLength, int[] argumentsPositions, string expectedResult)
        {
            var actualResult = templateBuilder.Generate(templateLength, argumentsPositions)
                .TemplateString.Replace(x => char.IsLetter(x) ? 'a' : x);
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }
}