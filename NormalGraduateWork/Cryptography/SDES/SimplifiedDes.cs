using System;
using System.Collections;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Validators;
using NormalGraduateWork.Extensions;

namespace NormalGraduateWork.Cryptography.SDES
{
    public class SDES
    {
        BitArray[,] S_Box1 = new BitArray[4, 4];
        BitArray[,] S_Box2 = new BitArray[4, 4];
        BitArray Master_key;

        public SDES(string _key)
        {
            Master_key = new BitArray(10);
            for (int i = 0; i < _key.Length; i++)
            {
                Master_key[i] = str2bin(_key[i]);
            }

            BitArray b0 = new BitArray(2);
            b0[0] = false;
            b0[1] = false;

            BitArray b1 = new BitArray(2);
            b0[0] = false;
            b0[1] = true;

            BitArray b2 = new BitArray(2);
            b0[0] = true;
            b0[1] = false;

            BitArray b3 = new BitArray(2);
            b0[0] = true;
            b0[1] = true;

            

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
            //---------------------
        }

        public byte Encrypt(byte block)
        {
            BitArray bits_block = byte2bits(block);
            BitArray[] keys = Generate_Keys();
            var ipResult = IP(bits_block);
            Console.WriteLine($"IP result {ipResult.ToNormalString()}");
            var fkResult = Fk(ipResult, keys[0]);
            Console.WriteLine($"First FK result {fkResult.ToNormalString()}");
            var switchResult = Switch(fkResult);
            Console.WriteLine($"Switch result {switchResult.ToNormalString()}");
            var secondFk = Fk(switchResult, keys[1]);
            Console.WriteLine($"Second FK result {secondFk.ToNormalString()}");
            var rip = RIP(secondFk);
            Console.WriteLine($"RIP result {rip.ToNormalString()}");
            return bits2byte(rip);
            //ciphertext = IP-1( fK2 ( SW (fK1 (IP (plaintext)))))
        }

        public byte Decrypt(byte block)
        {
            BitArray bits_block = byte2bits(block);
            BitArray[] keys = Generate_Keys();
            return bits2byte(RIP(Fk(Switch(Fk(IP(bits_block), keys[1])), keys[0])));
            //IP-1 ( fK1( SW( fK2( IP(ciphertext)))))
        }

        BitArray byte2bits(byte block)
        {
            string bits = decimal2binstr(block);
            BitArray result = new BitArray(8);
            for (int i = 0; i < bits.Length; i++)
            {
                result[i] = str2bin(bits[i]);
            }
            return result;
        }

        byte bits2byte(BitArray block)
        {
            string result = "";
            for (int i = 0; i < block.Length; i++)
            {
                result += bin2str(block[i]);
            }
            return binstr2decimal(result);
        }

        BitArray[] Generate_Keys()
        {
            BitArray [] keys = new BitArray[2];
            var p10 = P10(Master_key);
            
            BitArray[] temp = Split_Block(p10);

            var a = Circular_left_shift(temp[0], 1);
            var b = Circular_left_shift(temp[1], 1);
            
            
            keys[0] = P8(a, b);

            var c = Circular_left_shift(temp[0], 3);
            var d = Circular_left_shift(temp[1], 3);
            
            
            keys[1] = P8(c, d); //1 + 2 = 3
            
            return keys;
        }

        // decimal to binary string
        public string decimal2binstr(byte num)
        {
            string ret = "";
            for (int i = 0; i < 8; i++)
            {
                if (num % 2 == 1)
                    ret = "1" + ret;
                else
                    ret = "0" + ret;
                num >>= 1;
            }
            return ret;
        }

        // binary to decimal string
        public byte binstr2decimal(string binstr)
        {
            byte ret = 0;
            for (int i = 0; i < binstr.Length; i++)
            {
                ret <<= 1;
                if (binstr[i] == '1')
                    ret++;
            }
            return ret;
        }

        public string bin2str(bool input)
        {
            if (input)
                return "1";
            else
                return "0";
        }

        public bool str2bin(char bit)
        {
            if (bit == '0')
                return false;
            else if (bit == '1')
                return true;
            else
                throw new Exception("Key should be in binary format [0,1]");
        }

        //generates  permated array P10
        BitArray P10(BitArray key)
        {
            //0 1 2 3 4 5 6 7 8 9
            //2 4 1 6 3 9 0 8 7 5
            BitArray permutatedArray = new BitArray(10);

            permutatedArray[0] = key[2];
            permutatedArray[1] = key[4];
            permutatedArray[2] = key[1];
            permutatedArray[3] = key[6];
            permutatedArray[4] = key[3];
            permutatedArray[5] = key[9];
            permutatedArray[6] = key[0];
            permutatedArray[7] = key[8];
            permutatedArray[8] = key[7];
            permutatedArray[9] = key[5];

            return permutatedArray;
        }

        //generates permuted array P8
        BitArray P8(BitArray part1, BitArray part2)
        {
            //0 1 2 3 4 5 6 7
            //5 2 6 3 7 4 9 8
            //6 3 7 4 8 5 10 9
            BitArray permutatedArray = new BitArray(8);

            permutatedArray[0] = part2[0];//5
            permutatedArray[1] = part1[2];
            permutatedArray[2] = part2[1];//6
            permutatedArray[3] = part1[3];
            permutatedArray[4] = part2[2];//7
            permutatedArray[5] = part1[4];
            permutatedArray[6] = part2[4];//9
            permutatedArray[7] = part2[3];//8

            return permutatedArray;
        }

        BitArray P4(BitArray part1, BitArray part2)
        {
            //0 1 2 3
            //2 4 3 1
            //1 3 2 0
            BitArray permutatedArray = new BitArray(4);

            permutatedArray[0] = part1[1];
            permutatedArray[1] = part2[1];//3
            permutatedArray[2] = part2[0];//2
            permutatedArray[3] = part1[0];

            return permutatedArray;
        }

        BitArray EP(BitArray input)
        {
            //0 1 2 3
            //4 1 2 3 2 3 4 1
            //3 0 1 2 1 2 3 0
            BitArray permutatedArray = new BitArray(8);

            permutatedArray[0] = input[3];
            permutatedArray[1] = input[0];
            permutatedArray[2] = input[1];
            permutatedArray[3] = input[2];
            permutatedArray[4] = input[1];
            permutatedArray[5] = input[2];
            permutatedArray[6] = input[3];
            permutatedArray[7] = input[0];

            return permutatedArray;
        }

        //generates permuted text IP
        BitArray IP(BitArray plainText)
        {
            //0 1 2 3 4 5 6 7
            //1 5 2 0 3 7 4 6
            BitArray permutatedArray = new BitArray(8);

            permutatedArray[0] = plainText[1];
            permutatedArray[1] = plainText[5];
            permutatedArray[2] = plainText[2];
            permutatedArray[3] = plainText[0];
            permutatedArray[4] = plainText[3];
            permutatedArray[5] = plainText[7];
            permutatedArray[6] = plainText[4];
            permutatedArray[7] = plainText[6];

            return permutatedArray;
        }

        BitArray RIP(BitArray permutedText)
        {
            //0 1 2 3 4 5 6 7 
            //3 0 2 4 6 1 7 5

            BitArray permutatedArray = new BitArray(8);

            permutatedArray[0] = permutedText[3];
            permutatedArray[1] = permutedText[0];
            permutatedArray[2] = permutedText[2];
            permutatedArray[3] = permutedText[4];
            permutatedArray[4] = permutedText[6];
            permutatedArray[5] = permutedText[1];
            permutatedArray[6] = permutedText[7];
            permutatedArray[7] = permutedText[5];

            return permutatedArray;
        }

        BitArray Circular_left_shift(BitArray a, int bitNumber)
        {
            BitArray shifted = new BitArray(a.Length);
            int index = 0;
            for (int i = bitNumber; index < a.Length; i++)
            {
                shifted[index++] = a[i%a.Length]; 
            }
            return shifted;
        }

        BitArray[] Split_Block(BitArray block)
        {
            BitArray[] splited = new BitArray[2];
            splited[0] = new BitArray(block.Length / 2);
            splited[1] = new BitArray(block.Length / 2);
            int index = 0;

            for (int i = 0; i < block.Length/2; i++)
            {
                splited[0][i] = block[i];
            }
            for (int i = block.Length / 2; i < block.Length; i++)
            {
                splited[1][index++] = block[i];
            }
            return splited;
        }

        BitArray S_Boxes(BitArray input, int no)
        {
            BitArray[,] current_S_Box;

            if (no == 1)
                current_S_Box = S_Box1;
            else
                current_S_Box = S_Box2;

            var fi = binstr2decimal(bin2str(input[0]) + bin2str(input[3]));
            var si = binstr2decimal(bin2str(input[1]) + bin2str(input[2])); 
            var val=  current_S_Box[fi, si];
            return val;
        }

        BitArray F(BitArray right, BitArray sk)
        {
            BitArray[] temp = Split_Block(Xor(EP(right), sk));
            var sbox1 = S_Boxes(temp[0], 1);
            var sbox2 = S_Boxes(temp[1], 2);
            Console.WriteLine($"sbox1 {sbox1.ToNormalString()} sbox2 {sbox2.ToNormalString()}");
            return P4(sbox1, sbox2);
        }

        BitArray Fk(BitArray IP, BitArray key)
        {
            BitArray[] temp = Split_Block(IP);
            BitArray Left = Xor(temp[0], F(temp[1], key));
            BitArray joined = new BitArray(8);
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                joined[index++] = Left[i];
            }
            for (int i = 0; i < 4; i++)
            {
                joined[index++] = temp[1][i];
            }
            return joined;
        }

        BitArray Switch(BitArray input)
        {
            BitArray switched = new BitArray(8);
            int index = 0;
            for (int i = 4; index < input.Length; i++)
            {
                switched[index++] = input[i%input.Length];
            }
            return switched;
        }

        BitArray Xor(BitArray a, BitArray b)
        {
            return b.Xor(a);
        }
    }
    
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
            b0[0] = false;
            b0[1] = true;

            BitArray b2 = new BitArray(2);
            b0[0] = true;
            b0[1] = false;

            BitArray b3 = new BitArray(2);
            b0[0] = true;
            b0[1] = true;

            

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
            Console.WriteLine($"IP result {result.ToNormalString()}");
            
            result = Fk(result, subKeys[1]);
            Console.WriteLine($"First FK result {result.ToNormalString()}");
            
            result = Switch(result);
            Console.WriteLine($"Switch result {result.ToNormalString()}");
            
            result = Fk(result, subKeys[0]);
            Console.WriteLine($"Second FK result {result.ToNormalString()}");
            
            var rip = RIP(result);
            Console.WriteLine($"RIP result {rip.ToNormalString()}");
            return rip;
        }

        private BitArray InternalEncrypt(byte plain, BitArray[] subKeys)
        {
            var plainAsBitArray = plain.ToBitArray();
            var ipResult = IP(plainAsBitArray);
            Console.WriteLine($"IP result {ipResult.ToNormalString()}");
            var firstFkResult = Fk(ipResult, subKeys[0]);
            Console.WriteLine($"First FK result {firstFkResult.ToNormalString()}");
            var switchResult = Switch(firstFkResult);
            Console.WriteLine($"Switch result {switchResult.ToNormalString()}");
            var secondFkResult = Fk(switchResult, subKeys[1]);
            Console.WriteLine($"Second FK result {secondFkResult.ToNormalString()}");
            var rip = RIP(secondFkResult);
            Console.WriteLine($"RIP result {rip.ToNormalString()}");
            return rip;
        }

        // ok 
        private BitArray Switch(BitArray bitArray)
        {
            var leftPart = SimplifiedDesHelper.GetLeftPart(bitArray);
            var rightPart = SimplifiedDesHelper.GetRightPart(bitArray);
            return rightPart.Merge(leftPart);
        }

        // ok
        private BitArray Fk(BitArray afterIp, BitArray key)
        {
            var left = SimplifiedDesHelper.GetLeftPart(afterIp);
            var right = SimplifiedDesHelper.GetRightPart(afterIp);
            left = left.NormalXor(F(right, key));
            return left.Merge(right);
        }

        // ok
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
            
            Console.WriteLine($"sbox1 {leftPartSboxed.ToNormalString()} sbox2 {rightPartSboxed.ToNormalString()}");

            return SimplifiedDesHelper.P4(leftPartSboxed.Merge(rightPartSboxed));
        }

        // ok
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

        // ok
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

        // ok
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