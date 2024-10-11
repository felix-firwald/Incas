using Incas.Objects.Views.Controls;
using System.Collections.Generic;
using System.Data;

namespace Incas.Templates.Components
{
    public interface ITemplator
    {
        public void Replace(List<string> tags, List<string> values);

        public void GenerateDocument(List<IFiller> fillers);
        public void GenerateDocumentAsync(List<IFiller> fillers);
        public void CreateTable(string tag, DataTable dt);
        public List<string> FindAllTags();
        public string GetLogData();
        public List<string> FindTableTags(string tableName);
    }
}