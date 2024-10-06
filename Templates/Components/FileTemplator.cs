using Incas.Objects.Views.Controls;
using System.Collections.Generic;

namespace Incas.Templates.Components
{
    public class FileTemplator
    {
        private ITemplator templator;
        public FileTemplator(string name)
        {
            this.templator = name.EndsWith(".docx") ? new WordTemplator(name) : new ExcelTemplator(name);
        }
        public void GenerateDocument(List<FieldFiller> tagFillers, List<FieldTableFiller> tableFillers, bool async = true)
        {
            this.templator.GenerateDocument(tagFillers, tableFillers, async);
        }
        public List<string> FindAllTags()
        {
            return this.templator.FindAllTags();
        }
    }
}
