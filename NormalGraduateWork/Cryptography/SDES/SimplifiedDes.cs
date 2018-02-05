using System;
using System.Collections;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Validators;
using NormalGraduateWork.Extensions;

namespace NormalGraduateWork.Cryptography.SDES
{
    public class SimplifiedDes
    {
        private readonly SimplifiedDesSubKeysGenerator simplifiedDesSubKeysGenerator 
            = new SimplifiedDesSubKeysGenerator();

        private readonly BitArray[,] S_Box1 = new BitArray[4,4];

        private readonly BitArray[,] S_Box2 = new BitArray[4,4];

        public SimplifiedDes()
        {
            BitArray b0 = new BitArray(2);
            b0[0] = false;
            b0[1] = false;

            BitArray b1 = new BitArray(2);
            b1[0] = false;
            b1[1] = true;

            BitArray b2 = new BitArray(2);
            b2[0] = true;
            b2[1] = false;

            BitArray b3 = new BitArray(2);
            b3[0] = true;
            b3[1] = true;

            S_Box1[0, 0] = b1;
            S_Box1[0, 1] = b0;
            S_Box1[0, 2] = b3;
            S_Box1[0, 3] = b2;

            S_Box1[1, 0] = b3;
            S_Box1[1, 1] = b2;
            S_Box1[1, 2] = b1;
            S_Box1[1, 3] = b0;

            S_Box1[2, 0] = b0;
            S_Box1[2, 1] = b2;
            S_Box1[2, 2] = b1;
            S_Box1[2, 3] = b3;

            S_Box1[3, 0] = b3;
            S_Box1[3, 1] = b1;
            S_Box1[3, 2] = b3;
            S_Box1[3, 3] = b2;
            //---------------------
            S_Box2[0, 0] = b0;
            S_Box2[0, 1] = b1;
            S_Box2[0, 2] = b2;
            S_Box2[0, 3] = b3;

            S_Box2[1, 0] = b2;
            S_Box2[1, 1] = b0;
            S_Box2[1, 2] = b1;
            S_Box2[1, 3] = b3;

            S_Box2[2, 0] = b3;
            S_Box2[2, 1] = b0;
            S_Box2[2, 2] = b1;
            S_Box2[2, 3] = b0;

            S_Box2[3, 0] = b2;
            S_Box2[3, 1] = b1;
            S_Box2[3, 2] = b0;
            S_Box2[3, 3] = b3;
        }

        public byte Decrypt(byte cipher, BitArray key)
        {
            var subKeys = simplifiedDesSubKeysGenerator.Generate(key);
            return InternalDecrypt(cipher, subKeys).ToByte();
        }

        public byte Encrypt(byte plain, BitArray key)
        {
            var subKeys = simplifiedDesSubKeysGenerator.Generate(key);
            return InternalEncrypt(plain, subKeys).ToByte();
        }

        private BitArray InternalDecrypt(byte cipher, BitArray[] subKeys)
        {
            var cipherAsBitArray = cipher.ToBitArray();
            var result = IP(cipherAsBitArray);
            result = Fk(result, subKeys[1]);
            result = Switch(result);
            result = Fk(result, subKeys[0]);
            var rip = RIP(result);
            return rip;
        }

        private BitArray InternalEncrypt(byte plain, BitArray[] subKeys)
        {
            var plainAsBitArray = plain.ToBitArray();
            var ipResult = IP(plainAsBitArray);
            var firstFkResult = Fk(ipResult, subKeys[0]);
            var switchResult = Switch(firstFkResult);
            var secondFkResult = Fk(switchResult, subKeys[1]);
            var rip = RIP(secondFkResult);
            return rip;
        }

        private BitArray Switch(BitArray bitArray)
        {
            var leftPart = SimplifiedDesHelper.GetLeftPart(bitArray);
            var rightPart = SimplifiedDesHelper.GetRightPart(bitArray);
            return rightPart.Merge(leftPart);
        }

        private BitArray Fk(BitArray afterIp, BitArray key)
        {
            var left = SimplifiedDesHelper.GetLeftPart(afterIp);
            var right = SimplifiedDesHelper.GetRightPart(afterIp);
            left = left.NormalXor(F(right, key));
            return left.Merge(right);
        }

        private BitArray F(BitArray bitArray, BitArray key)
        {
            if (bitArray.Count != 4)
                throw new ArgumentException("F argument length should be 4 bits");

            var permuteAndExpand = new BitArray(8)
            {
                [0] = bitArray[3],
                [1] = bitArray[0],
                [2] = bitArray[1],
                [3] = bitArray[2],
                [4] = bitArray[1],
                [5] = bitArray[2],
                [6] = bitArray[3],
                [7] = bitArray[0]
            };
            var xoredWithKey = permuteAndExpand.NormalXor(key);
            
            var leftPart = SimplifiedDesHelper.GetLeftPart(xoredWithKey);
            var rightPart = SimplifiedDesHelper.GetRightPart(xoredWithKey);

            var leftPartSboxed = Sbox(leftPart, 1);
            var rightPartSboxed = Sbox(rightPart, 2);

            return SimplifiedDesHelper.P4(leftPartSboxed.Merge(rightPartSboxed));
        }

        private BitArray Sbox(BitArray bitArray, int number)
        {
            var sBox = number == 1 ? S_Box1 : S_Box2;
            var row = new BitArray(2) { [0] = bitArray[0], [1] = bitArray[3] };
            var column = new BitArray(2) { [0] = bitArray[1], [1] = bitArray[2] };
            var rowAsByte = row.ToByte();
            var columnAsByte = column.ToByte();
            var val = sBox[rowAsByte, columnAsByte];
            
            return val;
        }

        private BitArray IP(BitArray bitArray)
        {
            var permutatedArray = new BitArray(8)
            {
                [0] = bitArray[1],
                [1] = bitArray[5],
                [2] = bitArray[2],
                [3] = bitArray[0],
                [4] = bitArray[3],
                [5] = bitArray[7],
                [6] = bitArray[4],
                [7] = bitArray[6]
            };
            return permutatedArray;
        }

        private BitArray RIP(BitArray bitArray)
        {
            var permutatedArray = new BitArray(8)
            {
                [0] = bitArray[3],
                [1] = bitArray[0],
                [2] = bitArray[2],
                [3] = bitArray[4],
                [4] = bitArray[6],
                [5] = bitArray[1],
                [6] = bitArray[7],
                [7] = bitArray[5]
            };
            return permutatedArray;
        }
    }
}