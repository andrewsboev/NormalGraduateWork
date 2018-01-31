using System;
using System.Collections.Generic;
using System.IO;
using NormalGraduateWork.Cryptography.Aes16;

namespace NormalGraduateWork.Cryptography.Analysis
{
    public class Aes16Analyzer
    {
        public byte[] Analyze()
        {
            var result = BuildDifferentialTable();
            File.WriteAllText("D:\\data12332.txt", $"{result[0]} {result[1]} {result[2]} {Environment.NewLine}");
            var result2 = Build2(result[1]);
            File.AppendAllText("D:\\data12332.txt", $"{result2[0]} {result2[1]} {result2[2]} {Environment.NewLine}");
            return null;
        }

        private void Do(int diff1, int diff2, int diff3)
        {
            var firstList = new List<Tuple<int, int>>();
            var secondList = new List<Tuple<int, int>>();
            for (var i = 0; i <= 65535; ++i)
            {
                var first = i;
                var second = first ^ diff1;
                var firstAfter = FirstSbox(first);
                var secondAfter = FirstSbox(second);
                var xoredAfter = firstAfter ^ secondAfter;
                if (xoredAfter == diff2)
                    firstList.Add(Tuple.Create(first, (int)firstAfter));

                second = first ^ diff2;
                firstAfter = SecondSbox(first);
                secondAfter = SecondSbox(second);
                xoredAfter = firstAfter ^ secondAfter;
                if (xoredAfter == diff3)
                    secondList.Add(Tuple.Create(first, (int)firstAfter));
            }

            var numPairs = 10;
            var gp = FindGoodPair(diff1, diff3);
            for (var i = 0; i < firstList.Count; ++i)
            {
                for (var j = 0; j < secondList.Count; ++j)
                {
                    var k0 = (ushort) (gp.Item1 ^ firstList[i].Item1);
                    var k1 = (ushort) (firstList[i].Item2 ^ secondList[i].Item1);
                    var k3 = (ushort) (secondList[i].Item2 ^ gp.Item2);

                    for (var k = 0; k < numPairs; ++k)
                    {
                        // если что-то успешно не зашифровалось как надо, то плохо, не тот ключ
                    }
                    
                }
            }
            
        }
        
        private Tuple<int, int> FindGoodPair(int inDiff, int outDiff)
        {
            for (var i = 0; i <= 65536; ++i)
            {
                var first = (ushort)i;
                var second = (ushort)(first ^ inDiff);
                var firstEncrypted = BitConverter.ToUInt16(
                    new Aes16Encryptor().Encrypt(BitConverter.GetBytes(first), new byte[]{}), 0);
                var secondEncrypted = BitConverter.ToInt16(
                    new Aes16Encryptor().Encrypt(BitConverter.GetBytes(second), new byte[] { }), 0);
                var xored = firstEncrypted ^ secondEncrypted;
                if (xored == outDiff)
                    return Tuple.Create((int)first, (int)firstEncrypted);
            }

            return null;
        }

        private int[] Build2(int diff)
        {
            var values = new int[65536];
            var innerMaxIndex = -1;
            for (var i = 0; i <= 65535; ++i)
            {
                var firstComp = i;
                var secondComp = firstComp ^ diff;
                var firstAfterSbox = SecondSbox(firstComp);
                var secondAfterSbox = SecondSbox(secondComp);
                var xorAfterSbox = firstAfterSbox ^ secondAfterSbox;
                values[xorAfterSbox]++;
                if (innerMaxIndex == -1 || values[xorAfterSbox] > values[innerMaxIndex])
                    innerMaxIndex = xorAfterSbox;
            }
            return new int[] {diff, innerMaxIndex, values[innerMaxIndex]};
        }

        private int[] BuildDifferentialTable()
        {
            var maxInDiff = -1;
            var maxOutDiff = -1;
            var maxValue = -1;
            
            for (var inDiff = 0; inDiff <= 65535; ++inDiff)
            {
                Console.WriteLine($"inDiff = {inDiff}");
                var values = new int[65536];
                var innerMaxIndex = -1;
                for (var i = 0; i <= 65535; ++i)
                {
                    var firstComp = i;
                    var secondComp = firstComp ^ inDiff;
                    var firstAfterSbox = FirstSbox(firstComp);
                    var secondAfterSbox = FirstSbox(secondComp);
                    var xorAfterSbox = firstAfterSbox ^ secondAfterSbox;
                    values[xorAfterSbox]++;
                    if (innerMaxIndex == -1 || values[xorAfterSbox] > values[innerMaxIndex])
                        innerMaxIndex = xorAfterSbox;
                }

                if (maxValue == -1 || maxValue < values[innerMaxIndex])
                {
                    maxInDiff = inDiff;
                    maxOutDiff = innerMaxIndex;
                    maxValue = values[innerMaxIndex];
                }
            }

            return new[] {maxInDiff, maxOutDiff, maxValue};
        }

        private ushort FirstSbox(int val)
        {
            var asUshort = (ushort) val;
            var converted = BitConverter.GetBytes(asUshort);
            var nibbled = Aes16Helper.NibbleSubstitution(converted);
            var shifted = Aes16Helper.ShiftRow(nibbled);
            return BitConverter.ToUInt16(Aes16Helper.MixColumns(shifted), 0);
        }

        private ushort SecondSbox(int val)
        {
            var asUshort = (ushort) val;
            var converted = BitConverter.GetBytes(asUshort);
            var nibbled = Aes16Helper.NibbleSubstitution(converted);
            return BitConverter.ToUInt16(Aes16Helper.ShiftRow(nibbled), 0);
        }
    }
    
}