using System.Collections;
using NormalGraduateWork.Cryptography.SDES;

namespace NormalGraduateWork
{
    static class Program
    {
        static void Main(string[] args)
        {
           // var summary = BenchmarkRunner.Run<Aes16Analyzer>();
            //new Aes16Analyzer().Analyze();
            
            
       /*    var encryptor = new SDES("1000110010");
            var asByte = (byte) 0;
            var encrypted = encryptor.Encrypt(asByte);
       
         */ 
            
  var encryptor = new SimplifiedDes();
  var key = new BitArray(10)
  {
      [0] = true,
      [1] = false,
      [2] = false,
      [3] = false,
      [4] = true,
      [5] = true,
      [6] = false,
      [7] = false,
      [8] = true,
      [9] = false
  };
  var asByte = (byte) 0;
  var encrypted = encryptor.Encrypt(asByte, key);
}
}
}