using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable BuiltInTypeReferenceStyle

namespace NormalGraduateWork.Cryptography.Analysis.SimpleCipher
{
    using PlainCipherPair32 = PlainCipherPair<Int32>;
    
    public class SimpleCipherAnalyzer
    {
        private readonly Dictionary<int, int> sBox = new Dictionary<int, int>
        {
            {0, 3},
            {1, 14},
            {2, 1},
            {3, 10},
            {4, 4},
            {5, 9},
            {6, 5},
            {7, 6},
            {8, 8},
            {9, 11},
            {10, 15},
            {11, 2},
            {12, 13},
            {13, 12},
            {14, 0},
            {15, 7}
        };

        private readonly Dictionary<int, int> rsBox = new Dictionary<int, int>()
        {
            {3, 0},
            {14, 1},
            {1, 2},
            {10, 3},
            {4, 4},
            {9, 5},
            {5, 6},
            {6, 7},
            {8, 8},
            {11, 9},
            {15, 10},
            {2, 11},
            {13, 12},
            {12, 13},
            {0, 14},
            {7, 15}
        };

        private readonly byte key = 0b10011001;

        public byte? Crack()
        {
            var table = BuildDifferentialTable();
            var diffChars = FindGoodDifferentialCharacteristics(table);
            
            foreach (var diffChar in diffChars)
            {
                var possibleSuitableInputs = GetPossibleSuitableInputs(diffChar);
                if (possibleSuitableInputs == null || !possibleSuitableInputs.Any())
                    continue;
                
                var goodPairs = BuildGoodPair(diffChar);
                if (goodPairs == null || !goodPairs.Any())
                    continue;
                
                foreach (var x in possibleSuitableInputs)
                {
                    foreach (var goodPair in goodPairs)
                    {
                        var keyPart0 = x.Item1 ^ goodPair.Item1.Plain;
                        var keyPart1 = sBox[x.Item1] ^ goodPair.Item1.Cipher;

                        if (IsCorrectKey(keyPart0, keyPart1))
                        {
                            Console.WriteLine("FUCK");
                            return (byte) ((keyPart0 << 4) | keyPart1);
                        }
                    }
                }
            }
            return null;
        }

        private bool IsCorrectKey(int k0, int k1)
        {
            return (byte) ((k0 << 4) | k1) == key;
        }

        private List<Tuple<PlainCipherPair32, PlainCipherPair32>> BuildGoodPair(
            DifferentialCharacteristic characteristic)
        {
            var cipher = new OneRoundSimpleCipher.OneRoundSimpleCipher();
            var result = new List<Tuple<PlainCipherPair32, PlainCipherPair32>>();
            
            for (var i = 0; i < 16; ++i)
            {
                var firstPlain = i;
                var firstPlainBytes = BitConverter.GetBytes(firstPlain);
                Array.Reverse(firstPlainBytes);
                
                var secondPlain = firstPlain ^ characteristic.InputDifferential;
                var secondPlainBytes = BitConverter.GetBytes(secondPlain);
                Array.Reverse(secondPlainBytes);

                if (characteristic.InputDifferential == 4 && characteristic.OutputDifferential == 7)
                {
                    if (i == 11)
                        Console.WriteLine($"LOL {firstPlain} {secondPlain}");
                }

                var firstCipherBytes = cipher.Encrypt(firstPlainBytes, key);
                var secondCipherBytes = cipher.Encrypt(secondPlainBytes, key);
                firstCipherBytes[0] = firstCipherBytes[1] = firstCipherBytes[2] = 0;
                secondCipherBytes[0] = secondCipherBytes[1] = secondCipherBytes[2] = 0;
                
                firstCipherBytes[3] &= 0b00001111;
                secondCipherBytes[3] &= 0b00001111;
                
                Array.Reverse(firstCipherBytes);
                Array.Reverse(secondCipherBytes);
                var firstCipher = BitConverter.ToInt32(firstCipherBytes, 0);
                var secondCipher = BitConverter.ToInt32(secondCipherBytes, 0);
                
                if (characteristic.InputDifferential == 4 && characteristic.OutputDifferential == 7)
                {
                    if (i == 11)
                        Console.WriteLine($"LOL {firstCipher} {secondCipher}");
                }

                if ((firstCipher ^ secondCipher) == characteristic.OutputDifferential)
                {
                    var item1 = new PlainCipherPair32(firstPlain, firstCipher);
                    var item2 = new PlainCipherPair32(secondPlain, secondCipher);
                    result.Add(Tuple.Create(item1, item2));
                }
            }
            return result;
        }

        private List<Tuple<int, int>> GetPossibleSuitableInputs(
            DifferentialCharacteristic differentialCharacteristic)
        {
            var result = new List<Tuple<int, int>>();
            for (var i = 0; i < 16; ++i)
            {
                var secondValue = i ^ differentialCharacteristic.InputDifferential;
                if ((sBox[i] ^ sBox[secondValue]) == differentialCharacteristic.OutputDifferential)
                    result.Add(Tuple.Create(i, secondValue));
            }
            return result;
        }

        private DifferentialCharacteristic[] FindGoodDifferentialCharacteristics(int[,] table)
        {
            var list = new List<Tuple<Tuple<int, int>, int>>();
            for (var i = 0; i < 16; ++i)
                for (var j = 0; j < 16; ++j)
                    if (i != 0 || j != 0) 
                        list.Add(Tuple.Create(Tuple.Create(i, j), table[i, j]));
            list = list.OrderByDescending(x => x.Item2).ToList();
            return list
                .Where(x => x.Item2 == list.First().Item2)
                .Select(x => new DifferentialCharacteristic(x.Item1.Item1, x.Item1.Item2))
                .ToArray();
        }

        private int[,] BuildDifferentialTable()
        {
            var table = new int[16, 16];
            for (var i = 0; i < 16; ++i)
            {
                for (var j = 0; j < 16; ++j)
                {
                    var xor = i ^ j;
                    var xorAfterSbox = sBox[i] ^ sBox[j];
                    table[xor, xorAfterSbox]++;
                }
            }
            return table;
        }
    }
}