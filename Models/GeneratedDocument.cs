using Common;
using DocumentFormat.OpenXml.InkML;
using Incubator_2.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    public struct SGeneratedTag
    {
        public int tag { get; set; }
        public string value { get; set; }
    }
    public struct SGeneratedDocument
    {
        public int id;
        public int template;
        public string templateName;
        public DateTime generatedTime;
        public string fileName;
        public string reference;
        public List<SGeneratedTag> filledTags;
        public string filledTagsString;

        public List<SGeneratedTag> GetFilledTags()
        {
            if (filledTags == null)
            {
                filledTags = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SGeneratedTag>>(Cryptographer.DecryptString(filledTagsString));
            }            
            return filledTags;
        }
        public void SaveFilledTags(List<SGeneratedTag> tags)
        {
            filledTagsString = Cryptographer.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(tags));
        }

        public GeneratedDocument AsModel()
        {
            GeneratedDocument d = new();
            d.id = id;
            d.template = template;
            d.templateName = templateName;
            d.generatedTime = generatedTime;
            d.fileName = fileName;
            d.reference = reference;
            d.content = filledTagsString;
            return d;
        }
    }
    public class GeneratedDocument : Model
    {
        public int id { get; set; }
        public int template { get; set; }
        public string templateName { get; set; }
        public DateTime generatedTime { get; set; }
        public string fileName { get; set; }
        public string reference { get; set; }
        public string content { get; set; }

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
            d.filledTagsString = content;
            return d;
        }

        public List<string> GetAllUsedTemplates()
        {
            DataTable dt = StartCommand()
                .SelectUnique("templateName")
                .OrderByASC("templateName")
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
            generatedTime = DateTime.Now;
            reference = $"{generatedTime.ToString("yyMMddHHmmss")}{ProgramState.GenerateSlug(4)}";
            StartCommand()
                .Insert(new()
                {
                    {nameof(template), template.ToString() },
                    {nameof(templateName), templateName },
                    {nameof(generatedTime), generatedTime.ToString() },
                    {nameof(fileName), fileName },
                    {nameof(content), content },
                })
                .Accumulate();
        }
        public void RemoveRecord()
        {
            StartCommand()
                .Delete()
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }
        public void RemoveRecords(List<int> ids)
        {
            StartCommand()
                .Delete()
                .WhereIn("id", ids)
                .ExecuteVoid();
        }
        public void RemoveRecords(List<string> ids)
        {
            StartCommand()
                .Delete()
                .WhereIn("id", ids)
                .ExecuteVoid();
        }

        //public List<SGeneratedTag> GetFilledTags()
        //{
        //    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<SGeneratedTag>>(File.ReadAllText(RegistreCreatedJSON.GetReference(this.template, this.reference)));
        //}
        public void SaveFilledTags(List<SGeneratedTag> tags)
        {
            content = Cryptographer.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(tags));
        }
    }
}
