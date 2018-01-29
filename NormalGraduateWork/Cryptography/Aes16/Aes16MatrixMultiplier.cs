namespace NormalGraduateWork.Cryptography.Aes16
{
    public class Aes16MatrixMultiplier
    {
        private readonly GaloisField16 galoisField16 = new GaloisField16();
        
        public byte[,] Multiply(byte[,] first, byte[,] second)
        {
            var result = new byte[2, 2];
            for (var i = 0; i < 2; ++i)
            {
                for (var j = 0; j < 2; ++j)
                {
                    var sum = galoisField16.Multiply(first[i, 0], second[0, j])
                              ^ galoisField16.Multiply(first[i, 1], second[1, j]);
                    result[i, j] = (byte)sum;
                }
            }
            return result;
        }
    }
}