using System;
using System.Text;

namespace NormalGraduateWork.Extensions
{
    public static class StringExtensions
    {
        public static string Replace(this string str, Func<char, char> func)
        {
            var stringBuilder = new StringBuilder();
            foreach (var ch in str)
                stringBuilder.Append(func(ch));
            return stringBuilder.ToString();
        }
    }
}