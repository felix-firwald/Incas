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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Incubator_2.Models
{
    public struct SGeneratedTag
    {
        public int tag { get; set; }
        public string value { get; set; }
    }
    public enum DocumentStatus
    {
        Draft = 0,
        Created = 1,
        Approved = 2,
        Printed = 3,
        Done = 4
    }
    public struct SGeneratedDocument
    {
        public int id;
        public int template;
        public string templateName;
        public DateTime generatedTime;
        public string fileName;
        public string number;
        public string fullNumber;
        public DocumentStatus status { get; set; }
        public List<SGeneratedTag> filledTags;
        public string filledTagsString;
        public string author;
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
            d.number = number;
            d.fullNumber = fullNumber;
            d.status = status;
            d.content = filledTagsString;
            d.author = author;
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
        public string number { get; set; }
        public string fullNumber { get; set; }
        public DocumentStatus status { get; set; }
        public string content { get; set; }
        public string author { get; set; }

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
            d.number = number;
            d.fullNumber = fullNumber;
            d.status = status;
            d.author = author;
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
                this.status = ParseEnum(dr["status"], DocumentStatus.Draft);
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
            if (this.id > 0)
            {
                UpdateRecord();
                return;
            }
            author = ProgramState.CurrentUser.fullname;
            generatedTime = DateTime.Now;
            //reference = $"{generatedTime.ToString("yyMMddHHmmss")}{ProgramState.GenerateSlug(4)}";
            StartCommand()
                .Insert(new()
                {
                    {nameof(template), template.ToString() },
                    {nameof(templateName), templateName },
                    {nameof(generatedTime), generatedTime.ToString("yyyy.MM.dd HH:mm") },
                    {nameof(fileName), fileName },
                    {nameof(number), number },
                    {nameof(content), content },
                    {nameof(author), author }
                })
                .Accumulate();
        }
        public void UpdateRecord()
        {
            generatedTime = DateTime.Now;
            author = ProgramState.CurrentUser.fullname;
            StartCommand()
                .Update(nameof(content), content)
                .Update(nameof(fileName), fileName)
                .Update(nameof(author), author)
                .Update(nameof(number), number)
                .Update(nameof(fullNumber), fullNumber)
                .Update(nameof(status), status.ToString())
                .Update(nameof(templateName), templateName)
                .WhereEqual(nameof(id), id)
                .ExecuteVoid();
        }
        public bool CheckUniqueNumber()
        {
            return StartCommand().GetCount(nameof(fullNumber), tableName, $"fullNumber = '{fullNumber}' AND status IN ('Approved', 'Printed', 'Done')") < 2;
        }
        public void RemoveRecord()
        {
            switch (status)
            {
                case DocumentStatus.Draft:
                case DocumentStatus.Created:
                case DocumentStatus.Approved:
                    StartCommand()
                        .Delete()
                        .WhereEqual("id", id.ToString())
                        .ExecuteVoid();
                    break;
                case DocumentStatus.Printed:
                    ProgramState.ShowAccessErrorDialog($"Документ \"{fileName}\" помечен, как распечатанный. Распечатанный документ нельзя удалить!");
                    break;
                case DocumentStatus.Done:
                    ProgramState.ShowAccessErrorDialog($"Документ \"{fileName}\" помечен, как завершенный. Завершенный документ нельзя удалить!");
                    break;
            }
            
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
