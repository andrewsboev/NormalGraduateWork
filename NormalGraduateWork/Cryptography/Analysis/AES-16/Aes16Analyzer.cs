using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using NormalGraduateWork.Cryptography.Aes16;
using NormalGraduateWork.Extensions;
using NormalGraduateWork.Random;

namespace NormalGraduateWork.Cryptography.Analysis
{
    public class Aes16Analyzer
    {
        private readonly Aes16Encryptor aes16Encryptor = new Aes16Encryptor();
        private readonly Aes16SubKeysGenerator aes16SubKeysGenerator = new Aes16SubKeysGenerator();
        private byte[] key;
        private  IList<byte[]> subKeys;
        private  ushort firstSubKey;
        private  ushort secondSubKey;
        private  ushort thirdSubKey;
        
        // 1-7424-53255 (16384-4096)
        private readonly ushort[] inDiff1 = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}; 
        private readonly ushort[] inDiff2 = {7424, 49920, 33280, 13312, 26624, 16640, 23552, 40704, 43776, 10496};
        private readonly ushort[] outDiff2 = {53255, 20482, 61443, 8196, 4111, 16397, 32773, 45070, 36874, 12299};
        
        private readonly UsualRandom random = new UsualRandom();

        private readonly int numPairs = 128;
        private readonly Dictionary<ushort, ushort> firstSboxDict;
        private readonly Dictionary<ushort, ushort> secondSboxDict;
        
        public Aes16Analyzer()
        {
            key = new byte[2];
            new RNGCryptoServiceProvider().GetBytes(key);
            subKeys = aes16SubKeysGenerator.GetAllSubKeys(key);
            firstSubKey = subKeys[0].ToUInt16();
            secondSubKey = subKeys[1].ToUInt16();
            thirdSubKey = subKeys[2].ToUInt16();
            
            firstSboxDict = new Dictionary<ushort, ushort>();
            secondSboxDict = new Dictionary<ushort, ushort>();
            for (var i = 0; i <= 65535; ++i)
            {
                firstSboxDict[(ushort) i] = FirstSbox((ushort) i);
                secondSboxDict[(ushort) i] = SecondSbox((ushort) i);
            }
        }

        private Dictionary<Tuple<ushort, ushort>, ushort> encDict = new Dictionary<Tuple<ushort, ushort>, ushort>();
        
        public void Method()
        {
            var result = BuildDifferentialTable();
            var result2 = Build2(result);
            foreach (var x in result2)
                File.AppendAllText("D:\\data12332.txt", 
                    $"{x.Item1.Item1} {x.Item1.Item2} {x.Item2.Item1} {{{x.Item1.Item3},{x.Item2.Item2}}}{Environment.NewLine}");
        }
        
        public void Analyze()
        {
            var correctKey = 0;
            var attemptCount = 65536;

            /*encDict = new Dictionary<Tuple<ushort, ushort>, ushort>();
            for (var i = 0; i <= 65535; ++i)
            {
                var tkey = ((ushort) i).GetBytes();
                for (var j = 0; j < plainCipherPairs.Length; ++j)
                {
                    var plain = plainCipherPairs[j].Item1;
                    var cipher = aes16Encryptor.Encrypt(plain.GetBytes(), tkey);
                    encDict[Tuple.Create(plain, (ushort) i)] = cipher.ToUInt16();
                }
            }*/
            
            var inDiff1Value = inDiff1[0];
            var inDiff2Value = inDiff2[0];
            var outDiff1Value = outDiff2[0];

            var lists = GetLists(inDiff1Value, inDiff2Value, outDiff1Value);
            var goodPair = FindGoodPair(inDiff1Value, outDiff1Value);
            
            for (var index = 0; index < 10; ++index)
            {
                Console.WriteLine($"{index}-th index");
                var success = 0;
                var sw = Stopwatch.StartNew();
                
                for (var i = 0; i <= 65535; ++i)
                {
                    //Console.WriteLine($"key is {i}");
                    key = ((ushort) i).GetBytes();
                    subKeys = aes16SubKeysGenerator.GetAllSubKeys(key);
                    firstSubKey = subKeys[0].ToUInt16();
                    secondSubKey = subKeys[1].ToUInt16();
                    thirdSubKey = subKeys[2].ToUInt16();
                    var plainCipherPairs = BuildPlainCipherPairs();
                    
                    var suggestedKey = Do(lists.Item1, goodPair, plainCipherPairs);

                    var rightKey = suggestedKey != null &&
                                   !(suggestedKey[0] != firstSubKey || suggestedKey[1] != secondSubKey ||
                                     suggestedKey[2] != thirdSubKey);
                    if (rightKey)
                    {
                        success++;
                    }
                }
                Console.WriteLine(sw.Elapsed);
                Console.WriteLine($"success rate is {success}/65536");
            }
        }

        // ok
        private Tuple<ushort, ushort>[] BuildPlainCipherPairs()
        {
            var result = new Tuple<ushort, ushort>[numPairs];
            for (var i = 0; i < numPairs; ++i)
            {
                var plainText = new byte[2];
                random.GetBytes(plainText);
                var plainTextAsUshort = plainText.ToUInt16();
                var cipherText = aes16Encryptor.Encrypt(plainText, key);
                var cipherTextAsUshort = cipherText.ToUInt16();
                result[i] = Tuple.Create(plainTextAsUshort, cipherTextAsUshort);
            }
            return result;
        }

        private Tuple<Tuple<ushort, ushort>[], Tuple<ushort, ushort>[]> GetLists(ushort diff1, ushort diff2, ushort diff3)
        {
            var index1 = 0;
            var firstList = new Tuple<ushort, ushort>[16384];
            var index2 = 0;
            var secondList = new Tuple<ushort, ushort>[4096];
            for (var i = 0; i <= 65535; ++i)
            {
                var first = (ushort)i;
                var second = (ushort)(first ^ diff1);
                var firstAfter = firstSboxDict[first];
                var secondAfter = firstSboxDict[second];
                var xoredAfter = (ushort)(firstAfter ^ secondAfter);
                if (xoredAfter == diff2)
                    firstList[index1++] = Tuple.Create(first, firstAfter);

                second = (ushort)(first ^ diff2);
                firstAfter = secondSboxDict[first];
                secondAfter = secondSboxDict[second];
                xoredAfter = (ushort)(firstAfter ^ secondAfter);
                if (xoredAfter == diff3)
                    secondList[index2++] = Tuple.Create(first, firstAfter);
            }
            return Tuple.Create(firstList, secondList);
        }

        private List<ushort> Do(Tuple<ushort, ushort>[] firstList, Tuple<ushort, ushort> goodPair, 
            Tuple<ushort, ushort>[] plainCipherPairs)
        {
            for (var i = 0; i < firstList.Length; ++i)
            {
                var k0 = (ushort) (goodPair.Item1 ^ firstList[i].Item1);
                var badKey = false;

                foreach (var plainCipherPair in plainCipherPairs)
                {
                    var plainBytes = plainCipherPair.Item1.GetBytes();
                    
                    var encrypted = aes16Encryptor.Encrypt(plainBytes, k0.GetBytes());
                    var cipherAsUshort = encrypted.ToUInt16();
                    
                    if (cipherAsUshort != plainCipherPair.Item2)
                    {
                        badKey = true;
                        break;
                    }    
                }

                if (!badKey)
                {
                    var subkeys = aes16SubKeysGenerator.GetAllSubKeys(k0.GetBytes());
                    return new List<ushort>
                    {
                        k0,
                        subkeys[1].ToUInt16(),
                        subkeys[2].ToUInt16()
                    };
                }
            }
            return null;
        }
        
        private Tuple<ushort, ushort> FindGoodPair(ushort inDiff, ushort outDiff)
        {
            for (var i = 0; i <= 65535; ++i)
            {
                var first = (ushort)i;
                var second = (ushort)(first ^ inDiff);
                var firstBytes = first.GetBytes();
                var secondBytes = second.GetBytes();
                
                var firstEncrypted = aes16Encryptor.Encrypt(firstBytes, key).ToUInt16();
                var secondEncrypted = aes16Encryptor.Encrypt(secondBytes, key).ToUInt16();
                var xored = (ushort) (firstEncrypted ^ secondEncrypted);
                if (xored == outDiff)
                    return Tuple.Create(first, firstEncrypted);
            }
            return null;
        }
        
        public ushort FirstSbox(ushort val)
        {
            var converted = val.GetBytes();
            var nibbled = Aes16Helper.NibbleSubstitution(converted);
            var shifted = Aes16Helper.ShiftRow(nibbled);
            return Aes16Helper.MixColumns(shifted).ToUInt16();
        }

        public ushort SecondSbox(ushort val)
        {
            var converted = val.GetBytes();
            var nibbled = Aes16Helper.NibbleSubstitution(converted);
            return Aes16Helper.ShiftRow(nibbled).ToUInt16();
        }

        private List<Tuple<Tuple<int, int, int>, Tuple<int, int>>> Build2(List<Tuple<int, int, int>> list)
        {
            var result = new List<Tuple<Tuple<int, int, int>, Tuple<int, int>>>();
            foreach (var item in list)
            {
                var values = new int[65536];
                var innerMaxIndex = -1;
                for (var i = 0; i <= 65535; ++i)
                {
                    var firstComp = (ushort) i;
                    var secondComp = (ushort) (firstComp ^ item.Item2);
                    var firstAfterSbox = secondSboxDict[firstComp];
                    var secondAfterSbox = secondSboxDict[secondComp];
                    var xorAfterSbox = firstAfterSbox ^ secondAfterSbox;
                    values[xorAfterSbox]++;
                    if (innerMaxIndex == -1 || values[xorAfterSbox] > values[innerMaxIndex])
                        innerMaxIndex = xorAfterSbox;
                }

                var secondTuple = Tuple.Create(innerMaxIndex, values[innerMaxIndex]);
                var common = Tuple.Create(item, secondTuple);
                result.Add(common);
            }
            return result;
        }

        private List<Tuple<int, int, int>> BuildDifferentialTable()
        {
            var list = new List<Tuple<int, int, int>>();
            
            for (var inDiff = 1; inDiff <= 65535; ++inDiff)
            {
                Console.WriteLine($"inDiff = {inDiff}");
                var values = new int[65536];
                for (var i = 0; i <= 65535; ++i)
                {
                    var firstComp = (ushort)i;
                    var secondComp = (ushort) (firstComp ^ inDiff);
                    var firstAfterSbox = firstSboxDict[firstComp];
                    var secondAfterSbox = firstSboxDict[secondComp];
                    var xorAfterSbox = firstAfterSbox ^ secondAfterSbox;
                    values[xorAfterSbox]++;

                    if (list.Count < 10)
                        list.Add(Tuple.Create(inDiff, xorAfterSbox, values[xorAfterSbox]));
                    else
                        for (var k = 0; k < list.Count; ++k)
                            if (list[k].Item3 < values[xorAfterSbox])
                            {
                                list[k] = Tuple.Create(inDiff, xorAfterSbox, values[xorAfterSbox]);
                                break;
                            }
                }
            }
            foreach (var item in list)
                Console.WriteLine($"{item.Item1} {item.Item2} {item.Item3}");
            return list;
        }
    }
}