using Incas.Templates.Views.Controls;
using System.Collections.Generic;

namespace Incas.Templates.Components
{
    public class FileTemplator
    {
        ITemplator templator;
        public FileTemplator(string name)
        {
            if (name.EndsWith(".docx"))
            {
                this.templator = new WordTemplator(name);
            }
            else
            {
                this.templator = new ExcelTemplator(name);
            }
        }
        public void GenerateDocument(List<TagFiller> tagFillers, List<TableFiller> tableFillers, bool async = true)
        {
            this.templator.GenerateDocument(tagFillers, tableFillers, async);
        }
        public List<string> FindAllTags()
        {
            return this.templator.FindAllTags();
        }
    }
}
