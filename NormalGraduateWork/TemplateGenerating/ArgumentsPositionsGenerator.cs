using System;
using System.Collections.Generic;
using System.Linq;
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

            for (var i = 0; i < Math.Min(templateLength, 20); ++i)
                if (GenerateCombinations(templateLength + 1, firstCombination))
                {
                    var converted = firstCombination.Select(x => x - 1).ToList();
                    combinations.Add(converted);
                }
                else
                {
                    break;
                }

            return combinations;
        }

        private bool GenerateCombinations(int n, List<int> prevCombination)
        {
            var k = prevCombination.Count;
            for (var i = k - 1; i >= 0; --i)
            {
                if (prevCombination[i] < n - k + i + 1)
                {
                    ++prevCombination[i];
                    for (var j = i + 1; j < k; ++j)
                        prevCombination[j] = prevCombination[j - 1] + 1;
                    return true;
                }
            }

            return false;
        }
    }
}