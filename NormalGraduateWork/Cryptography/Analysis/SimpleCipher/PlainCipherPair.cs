namespace NormalGraduateWork.Cryptography.Analysis.SimpleCipher
{
    public class PlainCipherPair<T>
    {
        public T Plain { get; }
        public T Cipher { get; }

        public PlainCipherPair(T plain, T cipher)
        {
            Plain = plain;
            Cipher = cipher;
        }
    }
}