using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    public class GeneratedDocument : Model
    {
        public int id { get; set; }
        public int template { get; set; }
        public string templateName { get; set; }
        public DateTime generatedTime { get; set; }
        public string fileName { get; set; }
        public string reference { get; set; }
        public string destination { get; set; }

        public GeneratedDocument()
        {
            tableName = "GeneratedDocuments";
        }
        public void AddRecord()
        {
            StartCommand()
                .Insert(new()
                {
                    {nameof(template), template.ToString() },
                    {nameof(templateName), templateName },
                    {nameof(generatedTime), generatedTime.ToString() },
                    {nameof(fileName), fileName },
                    {nameof(reference), reference },
                    {nameof(destination), destination },
                })
                .ExecuteVoid();
        }
    }
}
