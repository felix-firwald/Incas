using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using IncasEngine.ObjectiveEngine.Types.Documents;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
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
        public FileTemplator(Template template, string name)
        {
            this.templator = name.EndsWith(".docx") ? new WordTemplator(template, name) : new ExcelTemplator(template, name);
        }
        public void GenerateDocument(Document doc)
        {
            this.templator.GenerateDocument(doc);
        }
        public List<string> FindAllTags()
        {
            return this.templator.FindAllTags();
        }
    }
}
