using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes.Jobs;
using NormalGraduateWork.Cryptography.Aes16;
using NormalGraduateWork.Random;
using NormalGraduateWork.TemplateGenerating;

namespace NormalGraduateWork
{
    public class TemplateStatistics
    {
        public string TemplateString { get; }
        public int SuccessfulAttempts { get; private set; }
        public int TotalAttempts { get; private set; }

        public TemplateStatistics(string templateString)
        {
            TemplateString = templateString;
        }
        
        public void AddAttempt(bool successful)
        {
            ++TotalAttempts;
            if (successful)
                ++SuccessfulAttempts;
        }
    }
    
    internal static class Program
    {
        private const int Aes16BlockLengthInBytes = 8;
        private const int Aes16BlockLengthInBits = 16;

        private static string TemplateFileName = @"D:\templates.txt";
        private static string PlainCipherFileName = @"D:\plaincipher.txt";
        private static TemplateListGenerator templateListGenerator;

        private static Aes16Encryptor aes16Encryptor;
        
        private static void Prepare()
        {
            var randomNumberGenerator = new RNGCryptoServiceProvider();
            var wrappedRandomNumberGenerator = new WrappedRandomNumberGenerator(randomNumberGenerator);
            var stringGenerator = new StringGenerator(wrappedRandomNumberGenerator);
            var templateBuilder = new TemplateBuilder(stringGenerator);
            var argumentsPositionsGenerator = new ArgumentsPositionsGenerator(wrappedRandomNumberGenerator);
            templateListGenerator = new TemplateListGenerator(templateBuilder, argumentsPositionsGenerator);
            aes16Encryptor = new Aes16Encryptor();
            
            //if (File.Exists(TemplateFileName))
            //    File.Delete(TemplateFileName);
            //if (File.Exists(PlainCipherFileName))
            //    File.Delete(PlainCipherFileName);
        }
        
        private static void Main(string[] args)
        {
            Prepare();

            var content = File.ReadAllLines(PlainCipherFileName);
            var templatesStats = new List<TemplateStatistics>();

            TemplateStatistics currentTemplateStats = null;
            foreach (var line in content)
            {
                if (line.StartsWith("TEMPLATE:"))
                {
                    if (currentTemplateStats != null) 
                        templatesStats.Add(currentTemplateStats);
                    currentTemplateStats = new TemplateStatistics(line.Split(' ').Skip(1).First());
                }
                else
                {
                    var parts = line.Split(' ');
                    var plain = parts[1];
                    var encrypted = parts[3];
                    var blockPairs = new List<Tuple<byte[], byte[]>>();
                    for (var i = 0; i < plain.Length; ++i)
                    {
                        var plainBytes = Encoding.Unicode.GetBytes(plain[i].ToString());
                        var encryptedBytes = Encoding.Unicode.GetBytes(encrypted[i].ToString());
                        blockPairs.Add(Tuple.Create(plainBytes, encryptedBytes));
                    }
                    
                    //
                    var success = false;
                    currentTemplateStats.AddAttempt(success);
                }
                
            }
            
            /*var key = new byte[] {0x56, 0xDE};
            var template = templateListGenerator.Generate(10, 1).First();
            var templateAsString = template.TemplateString;
            var plain = string.Format(templateAsString, "dfhsdfh");
            var plainBytes = Encoding.Unicode.GetBytes(plain);
            var encrypted = aes16Encryptor.Encrypt(plainBytes, key);
            var encryptedString = Encoding.Unicode.GetString(encrypted);
            */

            /*var some = new StringGenerator(new WrappedRandomNumberGenerator(new RNGCryptoServiceProvider())).GenerateSome();
            File.WriteAllLines(@"D:\some.txt", some);

            
            
            for (var templateLength = 20; templateLength <= 100; ++templateLength)
            {
                var template = templateListGenerator.Generate(templateLength, 1).First();
                var templateAsString = template.TemplateString;
                File.AppendAllLines(@"D:\templates.txt", new[] {$"TEMPLATE: {templateAsString}"});
                var plainCipherList = new List<Tuple<string, string>>();
                foreach (var arg in some)
                {
                    var plain = string.Format(templateAsString, arg);
                    var plainBytes = Encoding.Unicode.GetBytes(plain);
                    var encrypted = aes16Encryptor.Encrypt(plainBytes, key);
                    var encryptedString = Encoding.Unicode.GetString(encrypted);
                    plainCipherList.Add(Tuple.Create(plain, encryptedString));
                }
                File.AppendAllLines(PlainCipherFileName, plainCipherList.Select(p => $"Plain {p.Item1} Encrypted {p.Item2}"));
            }*/
        }
    }
}