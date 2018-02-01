namespace NormalGraduateWork.Cryptography.Analysis.SimpleCipher
{
    public class DifferentialCharacteristic
    {
        public int InputDifferential { get; }
        public int OutputDifferential { get; }
        
        public int Count { get; set; }

        public DifferentialCharacteristic(int inputDifferential, int outputDifferential)
        {
            InputDifferential = inputDifferential;
            OutputDifferential = outputDifferential;
        }
    }
}