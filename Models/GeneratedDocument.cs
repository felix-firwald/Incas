using Common;
using Incubator_2.Common;
using System;
using System.Collections.Generic;
using System.Data;

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
            if (this.filledTags == null)
            {
                this.filledTags = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SGeneratedTag>>(Cryptographer.DecryptString(this.filledTagsString));
            }
            return this.filledTags;
        }
        public void SaveFilledTags(List<SGeneratedTag> tags)
        {
            this.filledTagsString = Cryptographer.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(tags));
        }

        public GeneratedDocument AsModel()
        {
            GeneratedDocument d = new();
            d.id = this.id;
            d.template = this.template;
            d.templateName = this.templateName;
            d.generatedTime = this.generatedTime;
            d.fileName = this.fileName;
            d.number = this.number;
            d.fullNumber = this.fullNumber;
            d.status = this.status;
            d.content = this.filledTagsString;
            d.author = this.author;
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
            this.tableName = "GeneratedDocuments";
        }
        public SGeneratedDocument AsStruct()
        {
            SGeneratedDocument d = new();
            d.id = this.id;
            d.template = this.template;
            d.templateName = this.templateName;
            d.generatedTime = this.generatedTime;
            d.fileName = this.fileName;
            d.number = this.number;
            d.fullNumber = this.fullNumber;
            d.status = this.status;
            d.author = this.author;
            d.filledTagsString = this.content;
            return d;
        }

        public List<string> GetAllUsedTemplates()
        {
            DataTable dt = this.StartCommand()
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
            DataTable dt = this.StartCommand()
                .Select()
                .WhereEqual("templateName", templateName)
                .OrderByASC("generatedTime DESC, fileName")
                .Execute();
            List<SGeneratedDocument> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                this.status = ParseEnum(dr["status"], DocumentStatus.Draft);
                result.Add(this.AsStruct());
            }
            return result;
        }
        public List<SGeneratedDocument> GetAllDocuments()
        {
            DataTable dt = this.StartCommand()
                .Select()
                .OrderByDESC("template ASC, generatedTime")
                .Execute();
            List<SGeneratedDocument> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);

                result.Add(this.AsStruct());
            }
            return result;
        }
        public void AddRecord()
        {
            if (this.id > 0)
            {
                this.UpdateRecord();
                return;
            }
            this.author = ProgramState.CurrentUser.fullname;
            this.generatedTime = DateTime.Now;
            this.status = DocumentStatus.Created;
            //reference = $"{generatedTime.ToString("yyMMddHHmmss")}{ProgramState.GenerateSlug(4)}";
            this.StartCommand()
                .Insert(new()
                {
                    {nameof(this.template), this.template.ToString() },
                    {nameof(this.templateName), this.templateName },
                    {nameof(this.generatedTime), this.generatedTime.ToString("yyyy.MM.dd HH:mm") },
                    {nameof(this.fileName), this.fileName },
                    {nameof(this.number), this.number },
                    {nameof(this.fullNumber), this.fullNumber },
                    {nameof(this.status), this.status.ToString() },
                    {nameof(this.content), this.content },
                    {nameof(this.author), this.author }
                })
                .Accumulate();
        }
        public void UpdateRecord()
        {
            this.generatedTime = DateTime.Now;
            this.author = ProgramState.CurrentUser.fullname;
            this.StartCommand()
                .Update(nameof(this.content), this.content)
                .Update(nameof(this.fileName), this.fileName)
                .Update(nameof(this.author), this.author)
                .Update(nameof(this.number), this.number)
                .Update(nameof(this.fullNumber), this.fullNumber)
                .Update(nameof(this.status), this.status.ToString())
                .Update(nameof(this.templateName), this.templateName)
                .WhereEqual(nameof(this.id), this.id)
                .ExecuteVoid();
        }
        public bool CheckUniqueNumber()
        {
            return this.StartCommand().GetCount(nameof(this.fullNumber), this.tableName, $"fullNumber = '{this.fullNumber}' AND status IN ('Approved', 'Printed', 'Done')") < 2;
        }
        public void RemoveRecord()
        {
            switch (this.status)
            {
                case DocumentStatus.Draft:
                case DocumentStatus.Created:
                case DocumentStatus.Approved:
                    this.StartCommand()
                        .Delete()
                        .WhereEqual("id", this.id.ToString())
                        .ExecuteVoid();
                    break;
                case DocumentStatus.Printed:
                    ProgramState.ShowAccessErrorDialog($"Документ \"{this.fileName}\" помечен, как распечатанный. Распечатанный документ нельзя удалить!");
                    break;
                case DocumentStatus.Done:
                    ProgramState.ShowAccessErrorDialog($"Документ \"{this.fileName}\" помечен, как завершенный. Завершенный документ нельзя удалить!");
                    break;
            }

        }
        public void RemoveRecords(List<int> ids)
        {
            this.StartCommand()
                .Delete()
                .WhereIn("id", ids)
                .ExecuteVoid();
        }
        public void RemoveRecords(List<string> ids)
        {
            this.StartCommand()
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
            this.content = Cryptographer.EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(tags));
        }
    }
}
