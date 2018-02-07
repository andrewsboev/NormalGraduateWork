using System;
using System.Collections.Generic;

namespace NormalGraduateWork.TemplateGenerating
{
    public class TemplateListGenerator
    {
        private const int MaxTemplateLength = 10;
        private const int MaxArgumentsCount = 10;
        private readonly TemplateBuilder templateBuilder;
        private readonly ArgumentsPositionsGenerator argumentsPositionsGenerator;
        
        public TemplateListGenerator(TemplateBuilder templateBuilder, 
            ArgumentsPositionsGenerator argumentsPositionsGenerator)
        {
            this.templateBuilder = templateBuilder;
            this.argumentsPositionsGenerator = argumentsPositionsGenerator;
        }

        public List<Template> Generate(int length, int arguments)
        {
            return GenerateTemplates(length, arguments);
            
            var allTemplates = new List<Template>();
            for (var i = 10; i < MaxTemplateLength; ++i)
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
            var allArgumentsPositions = new List<List<int>>()
            {
                new List<int>() {0}
            };
            foreach (var argumentsPositions in allArgumentsPositions)
            {
                for (var i = 0; i < 1; ++i)
                {
                    var template = templateBuilder.Generate(templateLength, argumentsPositions.ToArray());
                    allTemplates.Add(template);
                }
            }
            return allTemplates;
        }
    }
}