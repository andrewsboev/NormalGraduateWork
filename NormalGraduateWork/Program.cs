using NormalGraduateWork.Cryptography;
using NormalGraduateWork.Cryptography.Analysis;

namespace NormalGraduateWork
{
    static class Program
    {
        static void Main(string[] args)
        {
            new Aes16Analyzer().Analyze();
        }
    }
}