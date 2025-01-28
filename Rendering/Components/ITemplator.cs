using Incas.Objects.Interfaces;
using Incas.Objects.Views.Controls;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Incas.Rendering.Components
{
    public interface ITemplator
    {
        public void Replace(List<string> tags, List<string> values);
        public void GenerateDocument(List<IFillerBase> fillers);
        public Task<bool> GenerateDocumentAsync(List<IFillerBase> fillers);
        public void CreateTable(string tag, DataTable dt);
        public List<string> FindAllTags();
        public string GetLogData();
        public List<string> FindTableTags(string tableName);
    }
}