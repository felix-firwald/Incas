using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Types.Documents;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Incas.Rendering.Components
{
    public interface ITemplator
    {
        public void Replace(List<string> tags, List<string> values);
        public void GenerateDocument(Document doc);
        public Task<bool> GenerateDocumentAsync(Document doc);
        public void CreateTable(string tag, DataTable dt);
        public List<string> FindAllTags();
        public string GetLogData();
        public List<string> FindTableTags(string tableName);
    }
}