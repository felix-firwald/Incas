using Incas.Objects.Views.Controls;
using System.Collections.Generic;
using System.Data;

namespace Incas.Templates.Components
{
    public interface ITemplator
    {
        public void Replace(List<string> tags, List<string> values, bool async = true);

        public void GenerateDocument(List<FieldFiller> tagFillers, List<FieldTableFiller> tableFillers, bool async = true);

        public void CreateTable(string tag, DataTable dt);
        public List<string> FindAllTags();
        //public void CreateByObject(Incas.Objects.Components.Object obj);
    }
}