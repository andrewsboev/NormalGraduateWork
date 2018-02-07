using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NormalGraduateWork.Random;

namespace NormalGraduateWork.TemplateGenerating
{
    public class StringGenerator
    {
        private const string RussianAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
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

        public List<string> GenerateList(int length)
        {
            var result = new List<string>();
            var start = new StringBuilder();
            for (var i = 0; i < length; ++i)
                start.Append(RussianAlphabet[0]);
            InternalGenerateList(length, 0, start, result);
            return result;
        }

        public List<string> GenerateSome()
        {
            var result = new List<string>();
            var random = new WrappedRandomNumberGenerator(new RNGCryptoServiceProvider());
            for (var i = 0; i < 1000; ++i)
            {
                var length = 1 + (Math.Abs(random.GetNextInt32()) % 11);
                var str = Generate(length);
                result.Add(str);
            }
            return result;
        }

        private void InternalGenerateList(int length, int currentIndex, StringBuilder currentString, List<string> result)
        {
            if (currentIndex >= length)
                return;
            
            foreach (var letter in RussianAlphabet)
            {
                currentString[currentIndex] = letter;
                if (currentIndex == length - 1)
                    result.Add(currentString.ToString());
                else
                    InternalGenerateList(length, currentIndex + 1, currentString, result);
            }
        }

        private char GetNextChar()
        {
            var nextCharOffset = Math.Abs(randomNumberGenerator.GetNextInt32()) % RussianAlphabet.Length;
            return RussianAlphabet[nextCharOffset];
        }
    }
}