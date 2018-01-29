using System.Security.Cryptography;

namespace NormalGraduateWork.Random
{
    public class UsualRandom : RandomNumberGenerator
    {
        private readonly System.Random generator = new System.Random();
            
        public override void GetBytes(byte[] data)
        {
            generator.NextBytes(data);
        }
    }
}