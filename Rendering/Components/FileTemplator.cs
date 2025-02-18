using DocumentFormat.OpenXml.Packaging;
using IncasEngine.ObjectiveEngine.Types.Documents;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Template = IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents.Template;

namespace Incas.Rendering.Components
{
    public class FileTemplator
    {
        internal const string ObjectReferenceProperty = "INCAS_OBJECT_REFERENCE";
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
        public async static Task<string> GetIdentifier(string path)
        {
            string result = null;
            if (path.EndsWith(".docx"))
            {
                result = await WordTemplator.GetMetaDataIdentifier(path);
            }
            else if (path.EndsWith(".xlsx"))
            {
                result = await ExcelTemplator.GetObjectReference(path);
            }
            return result;
        }
    }
}
