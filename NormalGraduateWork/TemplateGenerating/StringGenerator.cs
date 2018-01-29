using System.Text;
using NormalGraduateWork.Random;

namespace NormalGraduateWork.TemplateGenerating
{
    public class StringGenerator
    {
        private readonly WrappedRandomNumberGenerator randomNumberGenerator;
        
        public StringGenerator(WrappedRandomNumberGenerator randomNumberGenerator)
        {
            this.randomNumberGenerator = randomNumberGenerator;
        }

        public string Generate(int length)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < length; ++i)
            {
                var nextChar = GetNextChar();
                stringBuilder.Append(nextChar);
            }
            return stringBuilder.ToString();
        }

        private char GetNextChar()
        {
            var nextCharOffset = randomNumberGenerator.GetNextUInt32() % 26;
            var nextChar = (char) ('a' + nextCharOffset);
            return nextChar;
        }
    }
}