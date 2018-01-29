using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NormalGraduateWork.TemplateGenerating
{
    public class TemplateBuilder
    {
        private readonly StringGenerator stringGenerator;

        public TemplateBuilder(StringGenerator stringGenerator)
        {
            this.stringGenerator = stringGenerator;
        }

        public Template Generate(int templateLength, int[] argumentsPositions)
        {
            var fixedPartsLengths = GetFixedPartsLengths(templateLength, argumentsPositions);
            var templateString = BuildTemplate(argumentsPositions, 
                fixedPartsLengths);
            return new Template(templateString);
        }

        private string BuildTemplate(int[] argumentsPositions, int[] fixedPartsLengths)
        {
            var templateStringBuilder = new StringBuilder();
            var argumentIndex = 0;
            if (argumentsPositions.First() == 0)
            {
                templateStringBuilder.Append("{0}");
                argumentIndex++;
            }

            for (var i = 0;; ++i)
            {
                var indexLessThanFixedPartsCount = i < fixedPartsLengths.Length;
                var indexLessThanArgumentsCount = argumentIndex < argumentsPositions.Length;

                if (indexLessThanFixedPartsCount)
                    templateStringBuilder.Append(stringGenerator.Generate(fixedPartsLengths[i]));
                if (indexLessThanArgumentsCount)
                    templateStringBuilder.Append("{" + (argumentIndex++) + "}");
                if (!indexLessThanFixedPartsCount && !indexLessThanArgumentsCount)
                    break;
            }
            
            return templateStringBuilder.ToString();
        }

        private int[] GetFixedPartsLengths(int templateLength, int[] argumentsPositions)
        {
            var internalArgumentsPositions = new List<int>();
            if (argumentsPositions.First() != 0)
                internalArgumentsPositions.Add(0);
            internalArgumentsPositions.AddRange(argumentsPositions);
            if (argumentsPositions.Last() != templateLength)
                internalArgumentsPositions.Add(templateLength);

            var fixedPartsLengths = new int[internalArgumentsPositions.Count - 1];
            for (var i = 1; i < internalArgumentsPositions.Count; ++i)
                fixedPartsLengths[i - 1] = internalArgumentsPositions[i]
                                           - internalArgumentsPositions[i - 1];
            return fixedPartsLengths;
        }
    }
}