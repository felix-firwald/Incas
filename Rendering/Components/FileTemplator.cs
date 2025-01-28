using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using System.Collections.Generic;

namespace Incas.Rendering.Components
{
    public class FileTemplator
    {
        private ITemplator templator;
        public FileTemplator(string name)
        {
            this.templator = name.EndsWith(".docx") ? new WordTemplator(name) : new ExcelTemplator(name);
        }
        public FileTemplator(string template, string name)
        {
            this.templator = name.EndsWith(".docx") ? new WordTemplator(template, name) : new ExcelTemplator(template, name);
        }
        public void GenerateDocument(List<IFillerBase> fillers)
        {
            this.templator.GenerateDocument(fillers);
        }
        public List<string> FindAllTags()
        {
            return this.templator.FindAllTags();
        }
    }
}
