using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using NormalGraduateWork.Cryptography.Analysis.SimpleCipher;
using NormalGraduateWork.Random;

// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable SuggestBaseTypeForParameter

// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable ParameterTypeCanBeEnumerable.Local
// ReSharper disable BuiltInTypeReferenceStyle

namespace NormalGraduateWork.Cryptography
{
    using PlainCipherPair64 = PlainCipherPair<UInt64>;
    using ListOfDiffPairs = List<Tuple<PlainCipherPair<UInt64>, PlainCipherPair<UInt64>>>;
    
    public class Feal4Analyzer
    {
        private readonly Feal4SubKeysGenerator feal4SubKeysGenerator;
        private readonly Feal4Encryptor feal4Encryptor;
        private readonly WrappedRandomNumberGenerator randomNumberGenerator;

        private const int numberOfPlain = 12;
        private const UInt64 inputDiff1 = 0x8080000080800000;
        private const UInt64 inputDiff2 = 0x0000000080800000;
        private const UInt64 inputDiff3 = 0x0000000002000000;
        private const UInt32 outputDiff = 0x02000000;

        private readonly UInt32[] subKeys;

        public Feal4Analyzer(Feal4SubKeysGenerator feal4SubKeysGenerator, Feal4Encryptor feal4Encryptor)
        {
            this.feal4SubKeysGenerator = feal4SubKeysGenerator;
            this.feal4Encryptor = feal4Encryptor;
            randomNumberGenerator = new WrappedRandomNumberGenerator(
                new RNGCryptoServiceProvider());
            subKeys = this.feal4SubKeysGenerator.Generate();
            subKeys = new UInt32[] {3016387496, 478038835, 4216481020, 210561241, 2865391883, 1053470615};
        }

        public UInt32[] Analyze()
        {
            var commonStopWatch = Stopwatch.StartNew();
            
            Console.WriteLine("CRACKING ROUND 4");
            var fourthRoundStopWatch = Stopwatch.StartNew();
            var fourthRoundChosenPlaintexts = GetChosenPlaintexts(inputDiff1);
            var fourthRoundUndoneFinalOperation = UndoFinalOperation(fourthRoundChosenPlaintexts);
            for (var i = 0; i < fourthRoundUndoneFinalOperation.Count; ++i)
                Console.WriteLine(fourthRoundUndoneFinalOperation[i].Item1.Cipher);
            var fourthRoundSubKey = CrackLastRound(outputDiff, fourthRoundUndoneFinalOperation);
            Console.WriteLine($"4-th round cracking time: {fourthRoundStopWatch.Elapsed}");
            Console.WriteLine($"4-th round subkey: {fourthRoundSubKey}");
            
            Console.WriteLine("CRACKING ROUND 3");
            var thirdRoundStopWatch = Stopwatch.StartNew();
            var thirdRoundChosenPlaintexts = GetChosenPlaintexts(inputDiff2);
            var thirdRoundUndoneFinalOperation = UndoFinalOperation(thirdRoundChosenPlaintexts);
            var thirdRoundUndoneLastRound = UndoLastRound(thirdRoundUndoneFinalOperation, fourthRoundSubKey);
            var thirdRoundSubKey = CrackLastRound(outputDiff, thirdRoundUndoneLastRound);
            Console.WriteLine($"3-th round cracking time: {thirdRoundStopWatch.Elapsed}");
            
            Console.WriteLine("CRACKING ROUND 2");
            var secondRoundStopWatch = Stopwatch.StartNew();
            var secondRoundChosenPlaintexts = GetChosenPlaintexts(inputDiff3);
            var secondRoundUndoneFinalOperation = UndoFinalOperation(secondRoundChosenPlaintexts);
            var secondRoundUndoneLastRound = UndoLastRound(secondRoundUndoneFinalOperation, fourthRoundSubKey);
            var secondRoundUndonePrevLastRound = UndoLastRound(secondRoundUndoneLastRound, thirdRoundSubKey);
            var secondRoundSubKey = CrackLastRound(outputDiff, secondRoundUndonePrevLastRound);
            Console.WriteLine($"2-th round cracking time: {secondRoundStopWatch.Elapsed}");

            Console.WriteLine("CRACKING ROUND 1");
            var firstRoundStopWatch = Stopwatch.StartNew();
            var firstRoundUndoLastRound = UndoLastRound(secondRoundUndonePrevLastRound, secondRoundSubKey);

            UInt32 crackedSubKey0 = 0;
            UInt32 crackedSubKey4 = 0;
            UInt32 crackedSubKey5 = 0;

            for (UInt64 guessK0 = 0; guessK0 < (UInt64) UInt32.MaxValue; ++guessK0)
            {
                var guessK032 = (UInt32) guessK0;
                UInt32 guessK4 = 0;
                UInt32 guessK5 = 0;

                for (var i = 0; i < numberOfPlain; ++i)
                {
                    var firstPlainLeft = Feal4Helper.GetLeftHalf(
                        firstRoundUndoLastRound[i].Item1.Plain);
                    var firstPlainRight = Feal4Helper.GetRightHalf(
                        firstRoundUndoLastRound[i].Item1.Plain);
                    var firstCipherLeft = Feal4Helper.GetLeftHalf(
                        firstRoundUndoLastRound[i].Item1.Cipher);
                    var firstCipherRight = Feal4Helper.GetRightHalf(
                        firstRoundUndoLastRound[i].Item1.Cipher);
                    var tempY0 = 
                        Feal4Helper.Fbox(firstCipherRight ^ guessK032) ^ firstCipherLeft;
                    if (guessK4 == 0)
                    {
                        guessK4 = tempY0 ^ firstPlainLeft;
                        guessK5 = tempY0 ^ firstCipherRight ^ firstPlainRight;
                    }
                    else if (((tempY0 ^ firstPlainLeft) != guessK4)
                             || ((tempY0 ^ firstCipherRight ^ firstPlainRight) != guessK5))
                    {
                        guessK4 = 0;
                        guessK5 = 0;
                        break; 
                    }
                }
                
                if (guessK4 != 0)
                {
                    crackedSubKey0 = guessK032;
                    crackedSubKey4 = guessK4;
                    crackedSubKey5 = guessK5;
                    break;
                }
            }
            Console.WriteLine($"1-th round cracking time: {firstRoundStopWatch.Elapsed}");
            Console.WriteLine($"Overall elapsed time: {commonStopWatch.Elapsed}");
            
            if (subKeys[0] == crackedSubKey0)
                Console.WriteLine("0-th subkey GOOD");
            if (subKeys[1] == secondRoundSubKey)
                Console.WriteLine("1-th subkey GOOD");
            if (subKeys[2] == thirdRoundSubKey)
                Console.WriteLine("2-th subkey GOOD");
            if (subKeys[3] == fourthRoundSubKey)
                Console.WriteLine("3-th subkey GOOD");
            if (subKeys[4] == crackedSubKey4)
                Console.WriteLine("4-th subkey GOOD");
            if (subKeys[5] == crackedSubKey5)
                Console.WriteLine("5-th subkey GOOD");

            return new[]
            {
                crackedSubKey0, secondRoundSubKey, thirdRoundSubKey,
                fourthRoundSubKey, crackedSubKey4, crackedSubKey5
            };
        }

        // verified
        private UInt32 CrackLastRound(UInt32 outDiff, ListOfDiffPairs chosenPlaintexts)
        {
            Console.WriteLine("CRACK LAST ROUND");
            for (UInt64 fakeK = 0; fakeK < (UInt64)UInt32.MaxValue; ++fakeK)
            {
                var fakeK32 = (UInt32) fakeK;
                var score = GetScore(outDiff, chosenPlaintexts, fakeK32);
                if (score == numberOfPlain)
                    return fakeK32;
            }
            throw new ArgumentException();
        }

        // verified
        private static int GetScore(UInt32 outDiff, ListOfDiffPairs chosenPlaintexts, 
            UInt32 fakeK32)
        {
            var score = 0;
            for (var i = 0; i < numberOfPlain; ++i)
            {
                var cipherLeft = Feal4Helper.GetLeftHalf(chosenPlaintexts[i].Item1.Cipher) ^
                                 Feal4Helper.GetLeftHalf(chosenPlaintexts[i].Item2.Cipher);
                var cipherRight = Feal4Helper.GetRightHalf(chosenPlaintexts[i].Item1.Cipher) ^
                                  Feal4Helper.GetRightHalf(chosenPlaintexts[i].Item2.Cipher);

                var y = cipherRight;
                var z = cipherLeft ^ outDiff;

                var firstFakeLeft =
                    Feal4Helper.GetLeftHalf(chosenPlaintexts[i].Item1.Cipher);
                var firstFakeRight =
                    Feal4Helper.GetRightHalf(chosenPlaintexts[i].Item1.Cipher);
                var secondFakeLeft =
                    Feal4Helper.GetLeftHalf(chosenPlaintexts[i].Item2.Cipher);
                var secondFakeRight =
                    Feal4Helper.GetRightHalf(chosenPlaintexts[i].Item2.Cipher);

                var y0 = firstFakeRight;
                var y1 = secondFakeRight;

                var fakeInput0 = y0 ^ fakeK32;
                var fakeInput1 = y1 ^ fakeK32;

                var fakeOut0 = Feal4Helper.Fbox(fakeInput0);
                var fakeOut1 = Feal4Helper.Fbox(fakeInput1);
                var fakeDiff = fakeOut0 ^ fakeOut1;

                if (fakeDiff == z)
                    ++score;
                else
                    break;
            }
            return score;
        }
        
        // verified
        private static ListOfDiffPairs UndoLastRound(ListOfDiffPairs diffPairs, UInt32 subKey)
        {
            var result = new ListOfDiffPairs();
            for (var i = 0; i < numberOfPlain; ++i)
            {
                UInt32 firstCipherLeft = Feal4Helper.GetLeftHalf(diffPairs[i].Item1.Cipher);
                UInt32 firstCipherRight = Feal4Helper.GetRightHalf(diffPairs[i].Item1.Cipher);

                UInt32 secondCipherLeft = Feal4Helper.GetLeftHalf(diffPairs[i].Item2.Cipher);
                UInt32 secondCipherRight = Feal4Helper.GetRightHalf(diffPairs[i].Item2.Cipher);

                firstCipherLeft = firstCipherRight;
                secondCipherLeft = secondCipherRight;

                firstCipherRight = Feal4Helper.Fbox(firstCipherLeft ^ subKey) ^ (Feal4Helper.GetRightHalf(diffPairs[i].Item1.Cipher));
                secondCipherRight = Feal4Helper.Fbox(secondCipherLeft ^ subKey) ^ (Feal4Helper.GetRightHalf(diffPairs[i].Item2.Cipher));

                var firstObj = new PlainCipherPair<UInt64>(diffPairs[i].Item1.Plain,
                    Feal4Helper.Combine32BitHalfs(firstCipherLeft, firstCipherRight));
                var secondObj = new PlainCipherPair<UInt64>(diffPairs[i].Item2.Plain,
                    Feal4Helper.Combine32BitHalfs(secondCipherLeft, secondCipherRight));
                
                result.Add(Tuple.Create(firstObj, secondObj));
            }
            return result;
        }

        // verified
        private ListOfDiffPairs UndoFinalOperation(ListOfDiffPairs data)
        {
            var result = new ListOfDiffPairs();
            foreach (var item in data)
            {
                var firstLeftCipher = Feal4Helper.GetLeftHalf(item.Item1.Cipher);
                var firstRightCipher = Feal4Helper.GetRightHalf(item.Item1.Cipher) ^ firstLeftCipher;
                
                var secondLeftCipher = Feal4Helper.GetLeftHalf(item.Item2.Cipher);
                var secondRightCipher = Feal4Helper.GetRightHalf(item.Item2.Cipher) ^ secondLeftCipher;
                
                var firstRecombined = Feal4Helper.Combine32BitHalfs(firstLeftCipher, firstRightCipher);
                var secondRecombined = Feal4Helper.Combine32BitHalfs(secondLeftCipher, secondRightCipher);
                
                var firstObj = new PlainCipherPair64(item.Item1.Plain, firstRecombined);
                var secondObj = new PlainCipherPair64(item.Item2.Plain, secondRecombined);
                result.Add(Tuple.Create(firstObj, secondObj));
            }
            return result;
        }

        // verified
        private ListOfDiffPairs GetChosenPlaintexts(UInt64 diff)
        {
            var result = new ListOfDiffPairs();
            var firstPlains = new UInt64[]
            {
                12955285647861369651    ,
                18109648085315283161    ,
                12306764428762328983    ,
                17211297972828623338    ,
                15427241088469002237    ,
                17621018528392063319    ,
                5723879385805672826    ,
                6238609054337075805    ,
                7604817027291702491    ,
                2434021997319343249    ,
                14638823592962865666    ,
                6688019176124855016    
            };
            for (var i = 0; i < numberOfPlain; ++i)
            {
                var firstPlain = firstPlains[i];/*randomNumberGenerator.GetNextUInt64();*/
                var secondPlain = firstPlain ^ diff;
                var firstCipher = feal4Encryptor.Encrypt(firstPlain, subKeys);
                var secondCipher = feal4Encryptor.Encrypt(secondPlain, subKeys);

                var first = new PlainCipherPair64(firstPlain, firstCipher);
                var second = new PlainCipherPair64(secondPlain, secondCipher);
                result.Add(Tuple.Create(first, second));
                Console.WriteLine($"{firstPlain} {secondPlain} {firstCipher} {secondCipher}");
            }
            return result;
        }
    }
}