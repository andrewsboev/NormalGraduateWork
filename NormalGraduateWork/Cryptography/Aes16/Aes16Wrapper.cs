namespace NormalGraduateWork.Cryptography.Aes16
{
    public class Aes16Wrapper
    {
        private readonly Aes16Encryptor aes16Encryptor = new Aes16Encryptor();
        private readonly Aes16Decryptor aes16Decryptor = new Aes16Decryptor();
        
        public byte[] Encrypt(byte[] plainText, byte[] key)
        {
            return aes16Encryptor.Encrypt(plainText, key);
        }

        public byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            return aes16Decryptor.Decrypt(cipherText, key);
        }
    }
}