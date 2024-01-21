using Common;
using Incubator_2.Common;
using System;
using System.Collections.Generic;
using System.Data;
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
        public SGeneratedDocument AsStruct()
        {
            SGeneratedDocument d = new();
            d.id = id;
            d.template = template;
            d.templateName = templateName;
            d.generatedTime = generatedTime;
            d.fileName = fileName;
            d.reference = reference;
            d.destination = destination;
            return d;
        }
        public List<string> GetAllUsedTemplates()
        {
            DataTable dt = StartCommand()
                .SelectUnique("templateName")
                .Execute();
            List<string> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["templateName"].ToString());
            }
            return result;
        }
        public List<SGeneratedDocument> GetDocumentsByTemplate(string templateName)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("templateName", templateName)
                .OrderByASC("generatedTime DESC, fileName")
                .Execute();
            List<SGeneratedDocument> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                result.Add(AsStruct());
            }
            return result;
        }
        public List<SGeneratedDocument> GetAllDocuments()
        {
            DataTable dt = StartCommand()
                .Select()
                .OrderByDESC("template ASC, generatedTime")
                .Execute();
            List<SGeneratedDocument> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                result.Add(AsStruct());
            }
            return result;
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
        public void RemoveRecord()
        {
            StartCommand()
                .Delete()
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }
        public void RemoveRecords(List<string> ids)
        {
            StartCommand()
                .Delete()
                .WhereIn("id", ids)
                .ExecuteVoid();
        }
    }
}
