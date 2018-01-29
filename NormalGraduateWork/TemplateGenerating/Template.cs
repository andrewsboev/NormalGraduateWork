namespace NormalGraduateWork.TemplateGenerating
{
    public class Template
    {
        public string TemplateString { get; }

        public Template(string templateString)
        {
            TemplateString = templateString;
        }

        public string Format(string[] arguments)
        {
            return string.Format(TemplateString, arguments);
        }
    }
}