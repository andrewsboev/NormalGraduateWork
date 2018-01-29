using System;
using System.Collections.Generic;

namespace NormalGraduateWork.TemplateGenerating
{
    public class TemplateListGenerator
    {
        private const int MaxTemplateLength = 100;
        private const int MaxArgumentsCount = 100;
        private readonly TemplateBuilder templateBuilder;
        private readonly ArgumentsPositionsGenerator argumentsPositionsGenerator;
        
        public TemplateListGenerator(TemplateBuilder templateBuilder, 
            ArgumentsPositionsGenerator argumentsPositionsGenerator)
        {
            this.templateBuilder = templateBuilder;
            this.argumentsPositionsGenerator = argumentsPositionsGenerator;
        }

        public List<Template> Generate()
        {
            var allTemplates = new List<Template>();
            for (var i = 1; i < MaxTemplateLength; ++i)
            {
                var templatesWithLength = GenerateTemplates(i);
                allTemplates.AddRange(templatesWithLength);
            }
            return allTemplates;
        }

        private List<Template> GenerateTemplates(int templateLength)
        {
            var allTemplates = new List<Template>();
            for (var i = 1; i <= Math.Min(templateLength, MaxArgumentsCount); ++i)
            {
                var templatesWithArgumentsCount = GenerateTemplates(templateLength, i);
                allTemplates.AddRange(templatesWithArgumentsCount);
            }
            return allTemplates;
        }

        private List<Template> GenerateTemplates(int templateLength, int argumentsCount)
        {
            var allTemplates = new List<Template>();
            var allArgumentsPositions = argumentsPositionsGenerator.Generate(argumentsCount,
                templateLength);
            foreach (var argumentsPositions in allArgumentsPositions)
            {
                var template = templateBuilder.Generate(templateLength,
                    argumentsPositions.ToArray());
                allTemplates.Add(template);
            }

            return allTemplates;
        }
    }
}