using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NormalGraduateWork.Random;

namespace NormalGraduateWork.TemplateGenerating
{
    public class ArgumentsPositionsGenerator
    {
        private WrappedRandomNumberGenerator wrappedRandomNumberGenerator;

        public ArgumentsPositionsGenerator(
            WrappedRandomNumberGenerator wrappedRandomNumberGenerator)
        {
            this.wrappedRandomNumberGenerator = wrappedRandomNumberGenerator;
        }

        public List<List<int>> Generate(int numberOfArguments, int templateLength)
        {
            var combinations = new List<List<int>>();
            var firstCombination = Enumerable.Range(1, numberOfArguments).ToList();
            combinations.Add(firstCombination.Select(x => x - 1).ToList());
            
            for (var i = 0; i < 10; ++i)
            {
                if (GenerateCombinations(templateLength + 1, firstCombination))
                {
                    var converted = firstCombination.Select(x => x - 1).ToList();
                    combinations.Add(converted);
                }
                else break;
            }

            return combinations;
        }

        private bool GenerateCombinations(int n, List<int> prevCombination)
        {
            var k = prevCombination.Count;
            for (var i = k-1; i >= 0; --i)
                if (prevCombination[i] < n - k + i + 1) {
                    ++prevCombination[i];
                    for (var j = i + 1; j < k; ++j)
                        prevCombination[j] = prevCombination[j-1]+1;
                    return true;
                }
            return false;
        }

        private int GetNumberOfCombinations(int n, int k)
        {
            var nFact = Factorial(n);
            var kFact = Factorial(k);
            var nMinuskFact = Factorial(n - k);
            return nFact / (kFact * nMinuskFact);
        }

        private int Factorial(int x)
        {
            var result = BigInteger.One;
            for (var i = 2; i <= x; ++i)
                result = result * i;
            return (int) result;
        }
    }
}