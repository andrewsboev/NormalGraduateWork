using System;
using BenchmarkDotNet.Running;
using NormalGraduateWork.Cryptography;
using NormalGraduateWork.Cryptography.Aes16;
using NormalGraduateWork.Cryptography.Analysis;

namespace NormalGraduateWork
{
    static class Program
    {
        static void Main(string[] args)
        {
           // var summary = BenchmarkRunner.Run<Aes16Analyzer>();
            new Aes16Analyzer().Analyze();
        }
    }
}