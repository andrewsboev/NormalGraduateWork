using NormalGraduateWork.Cryptography;

namespace NormalGraduateWork
{
    static class Program
    {
        static void Main(string[] args)
        {
            var subKeys = new Feal4SubKeysGenerator();
            var feal4Encryptor = new Feal4Encryptor();
            var feal4Analyzer = new Feal4Analyzer(subKeys, feal4Encryptor);
            var result = feal4Analyzer.Analyze();
        }
    }
}