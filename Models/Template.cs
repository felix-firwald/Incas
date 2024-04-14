using Common;
using Incubator_2.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using static Common.Query;

namespace Models
{
    public struct TemplateSettings
    {
        public string Validation;
        public string OnSaving;
        public string NumberPrefix;
        public string NumberPostfix;
    }
    public enum TemplateType
    {
        Word,
        Text,
        Excel
    }
    public struct STemplate
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string suggestedPath { get; set; }
        public int parent { get; set; }
        public TemplateType type { get; set; }
        public string settings { get; set; }

        public Template AsModel()
        {
            Template template = new();
            template.id = id;
            template.name = name;
            template.path = path;
            template.suggestedPath = suggestedPath;
            template.parent = parent;
            template.type = type;
            template.settings = settings;
            return template;
        }
    }
    public class Template : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string suggestedPath { get; set; }
        public TemplateType type { get; set; }
        public int parent { get; set; }
        public string settings { get; set; }

        public Template()
        {
            tableName = "Templates";
        }
        public Template(int newId)
        {
            tableName = "Templates";
            GetTemplateById(newId);
        }
        public STemplate AsStruct()
        {
            STemplate template = new();
            template.id = id;
            template.name = name;
            template.path = path;
            template.suggestedPath = suggestedPath;
            template.parent = parent;
            template.type = type;
            template.settings = settings;
            return template;
        }

        public List<STemplate> Parse(DataTable dt)
        {
            List<STemplate> resulting = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                this.type = (TemplateType)Enum.Parse(typeof(TemplateType), dr["type"].ToString());
                resulting.Add(this.AsStruct());
            }
            return resulting;
        }

        public Template GetTemplateById(int id)
        {
            DataRow dr = StartCommand()
                .Select()
                .WhereEqual("id", id.ToString())
                .ExecuteOne();
            if (dr != null)
            {
                this.Serialize(dr);
                this.type = (TemplateType)Enum.Parse(typeof(TemplateType), dr["type"].ToString(), true);
                return this;
            }
            else
            {
                ProgramState.ShowErrorDialog("Шаблон с таким идентификатором не был найден.", "Ошибка");
                return null;
            }

        }
        private List<STemplate> GetAllTemplatesBy(TemplateType tt, string cat)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("type", tt.ToString())
                .WhereEqual("suggestedPath", cat)
                .WhereNULL("parent")
                .OrderByASC("name")
                .Execute();            
            return Parse(dt);
        }
        public List<STemplate> GetAllWordExcelTemplates(string cat)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("suggestedPath", cat)
                .WhereIn("type", new List<string> { "Excel", "Word"})             
                .WhereNULL("parent")
                .OrderByASC("name")
                .Execute();
            return Parse(dt);
        }
        public List<STemplate> GetAllWordTemplates()
        {
            return GetAllByType(TemplateType.Word);
        }
        public List<STemplate> GetAllTextTemplates()
        {
            return GetAllByType(TemplateType.Text);
        }
        private List<STemplate> GetAllByType(TemplateType type)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("type", type.ToString())
                .OrderByASC("name")
                .Execute();
            List<STemplate> resulting = new();
            return Parse(dt);
        }
        public Template GetTemplateByName(string nameOf)
        {
            DataRow dt = StartCommand()
                    .Select()
                    .WhereEqual("name", nameOf)
                    .ExecuteOne();
            this.Serialize(dt);
            this.type = (TemplateType)Enum.Parse(typeof(TemplateType), dt["type"].ToString());
            return this;
        }
        public List<STemplate> GetWordTemplates(string category)
        {
            return GetAllTemplatesBy(TemplateType.Word, category);
        }
        public List<STemplate> GetExcelTemplates()
        {
            return GetAllTemplatesBy(TemplateType.Excel, "");
        }
        public List<string> GetCategories()
        {
            DataTable dt = StartCommand()
                .SelectUnique("suggestedPath")
                .OrderByASC("suggestedPath")
                .Execute();
            List<string> categories = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                categories.Add(dr[0].ToString());
            }
            return categories;
        }

        public void AddTemplate(bool isChild)
        {
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "name", name },
                    { "path", path },
                    { "parent", isChild? parent.ToString(): Query.Null }, // раньше тут было просто null а теперь будет 'null'
                    { "suggestedPath", isChild? Query.Null : suggestedPath },
                    { "type", type.ToString() } ,
                    { "settings", settings }
                })
                .ExecuteVoid();
            this.id = int.Parse(
                        StartCommand()
                            .Select()
                            .WhereEqual("name", name)
                            .WhereEqual("path", path)
                            .OrderByDESC("id")
                            .ExecuteOne()["id"].ToString()
                );
        }
        public void UpdateTemplate()
        {
            StartCommand()
                .Update("name", name)
                .Update("path", path)
                .Update("suggestedPath", suggestedPath)
                .Update("settings", settings)
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }

        public void RemoveTemplate()
        {
            if (ProgramState.IsWorkspaceOpened())
            {
                StartCommand()
                    .Delete()
                    .WhereEqual("id", id.ToString())
                    .Execute();
            }
        }
        public List<STemplate> GetAllChildren(List<int> ids)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereIn("parent", ids)
                .OrderByASC("name")
                .Execute();
            return Parse(dt);
        }
        public Template GetChild(string name)
        {
            DataRow dr = StartCommand()
                .Select()
                .WhereEqual("parent", this.id.ToString())
                .WhereEqual("name", name)
                .ExecuteOne();
            this.Serialize(dr);
            return this;

        }
        public TemplateSettings GetTemplateSettings()
        {
            try
            {
                return JsonConvert.DeserializeObject<TemplateSettings>(Cryptographer.DecryptString(this.settings));
            }
            catch (Exception)
            {
                return new();
            }
        }
        public void SaveTemplateSettings(TemplateSettings ts)
        {
            this.settings = Cryptographer.EncryptString(JsonConvert.SerializeObject(ts));
        }
    }
}
