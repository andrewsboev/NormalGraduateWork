using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using NormalGraduateWork.Cryptography.Aes16;
using NormalGraduateWork.Extensions;

namespace NormalGraduateWork.Cryptography.Analysis
{
    public class Aes16Analyzer
    {
        private readonly Aes16Encryptor aes16Encryptor = new Aes16Encryptor();
        private readonly Aes16SubKeysGenerator aes16SubKeysGenerator = new Aes16SubKeysGenerator();
        private byte[] key;
        private IList<byte[]> subKeys;
        private readonly ushort firstSubKey;
        private readonly ushort secondSubKey;
        private readonly ushort thirdSubKey;
        
        private readonly Dictionary<ushort, ushort> firstSboxDict;
        private readonly Dictionary<ushort, ushort> secondSboxDict;

        private readonly Tuple<ushort, ushort>[] pcp;
        private readonly Tuple<ushort, ushort> rgp;
        
        public Aes16Analyzer(byte[] key, Tuple<ushort, ushort>[] pcp, Tuple<ushort, ushort> rgp)
        {
            this.pcp = pcp;
            this.rgp = rgp;
            this.key = key;
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
        
        public bool Analyze(ushort inDiff)
        {
            var inDiff1Value = inDiff;
            var inDiff2Value = GetBestOutputDiffForDiff(inDiff);

            var lists = GetLists(inDiff1Value, inDiff2Value);
            var goodPair = rgp;
            
            var plainCipherPairs = pcp;
            var suggestedKey = Do(lists, goodPair, plainCipherPairs);
            return suggestedKey != null &&
                  !(suggestedKey[0] != firstSubKey || suggestedKey[1] != secondSubKey || suggestedKey[2] != thirdSubKey);
        }

        // getlists - пары внутренних (после ключа) входов в S-BOX
        private ushort[] GetLists(ushort diff1, ushort diff2)
        {
            var index1 = 0;
            var firstList = new ushort[16384];
            for (var i = 0; i <= 65535; ++i)
            {
                var first = (ushort)i;
                var second = (ushort)(first ^ diff1);
                var firstAfter = firstSboxDict[first];
                var secondAfter = firstSboxDict[second];
                var xoredAfter = (ushort)(firstAfter ^ secondAfter);
                if (xoredAfter == diff2)
                    firstList[index1++] = first;
            }
            return firstList;
        }

        private List<ushort> Do(ushort[] firstList, Tuple<ushort, ushort> goodPair, 
            Tuple<ushort, ushort>[] plainCipherPairs)
        {
            for (var i = 0; i < firstList.Length; ++i)
            {
                var k0 = (ushort) (goodPair.Item1 ^ firstList[i]);
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

        private ushort GetBestOutputDiffForDiff(int diff)
        {
            var values = new int[65536];
            var maxIndex = -1;
            for (var i = 0; i <= 65535; ++i)
            {
                var firstComp = (ushort)i;
                var secondComp = (ushort) (firstComp ^ diff);
                var firstAfterSbox = firstSboxDict[firstComp];
                var secondAfterSbox = firstSboxDict[secondComp];
                var xorAfterSbox = firstAfterSbox ^ secondAfterSbox;
                values[xorAfterSbox]++;

                if (maxIndex == -1 || values[maxIndex] < values[xorAfterSbox])
                    maxIndex = xorAfterSbox;
            }
            return (ushort) values[maxIndex];
        }

        /*
         *
         * // ok
        private Tuple<ushort, ushort>[] BuildPlainCipherPairs()
        {
            return pcp;
            /*var result = new Tuple<ushort, ushort>[numPairs];
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
         
         private Tuple<ushort, ushort> FindGoodPair(ushort inDiff, ushort outDiff)
        {
            return rgp;
            /*for (var i = 0; i <= 65535; ++i)
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
        
        public void Method()
       {
           var result = BuildDifferentialTable();
           var result2 = Build2(result);
           foreach (var x in result2)
               File.AppendAllText("D:\\data12332.txt", 
                   $"{x.Item1.Item1} {x.Item1.Item2} {x.Item2.Item1} {{{x.Item1.Item3},{x.Item2.Item2}}}{Environment.NewLine}");
       }*/
    }
}