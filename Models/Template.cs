using Common;
using IncasEngine.TemplateManager;
using Incubator_2.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Models
{
    public class TemplatePort
    {
        public Template SourceTemplate { get; set; }
        public List<Tag> Tags { get; set; }
        public string Source { get; set; }

        public void FillData(Template temp, bool applyParent)
        {
            this.SourceTemplate = temp;
            using (Tag tag = new())
            {
                if (applyParent)
                {
                    this.Tags = tag.GetAllTagsByTemplate(this.SourceTemplate.id, this.SourceTemplate.parent);
                }
                else
                {
                    this.Tags = tag.GetAllTagsByTemplate(this.SourceTemplate.id);
                }
            }
            foreach (Tag tag in this.Tags)
            {
                tag.id = 0;
            }
            this.SourceTemplate.id = 0;
            this.SourceTemplate.parent = "";
            switch (this.SourceTemplate.type)
            {
                case TemplateType.Word:
                    WordTemplator wt = new(ProgramState.GetFullnameOfWordFile(this.SourceTemplate.path));
                    this.Source = wt.Serialize();
                    break;
                case TemplateType.Excel:
                    this.Source = File.ReadAllText(ProgramState.GetFullnameOfExcelFile(this.SourceTemplate.path), System.Text.Encoding.UTF8);
                    break;
            }
        }
        public void ToFile(string folder)
        {
            File.WriteAllText($"{folder}\\{this.SourceTemplate.name}.tinc", JsonConvert.SerializeObject(this), System.Text.Encoding.UTF8);
        }
    }
    public struct TemplateSettings
    {
        public string Validation;
        public string OnSaving;
        public string NumberPrefix;
        public string NumberPostfix;
        public string FileNameTemplate;
        public bool RequiresSave;
        public bool PreventSave;
        public string OnOpening;
    }
    public struct STemplate
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string suggestedPath { get; set; }
        public string parent { get; set; }
        public TemplateType type { get; set; }
        public string settings { get; set; }

        public Template AsModel()
        {
            Template template = new();
            template.id = this.id;
            template.name = this.name;
            template.path = this.path;
            template.suggestedPath = this.suggestedPath;
            template.parent = this.parent;
            template.type = this.type;
            template.settings = this.settings;
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
        public string parent { get; set; }
        public string settings { get; set; }

        public Template()
        {
            this.tableName = "Templates";
        }
        public Template(int newId)
        {
            this.tableName = "Templates";
            this.GetTemplateById(newId);
        }
        public STemplate AsStruct()
        {
            STemplate template = new();
            template.id = this.id;
            template.name = this.name;
            template.path = this.path;
            template.suggestedPath = this.suggestedPath;
            template.parent = this.parent;
            template.type = this.type;
            template.settings = this.settings;
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
            DataRow dr = this.StartCommand()
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
            DataTable dt = this.StartCommand()
                .Select()
                .WhereEqual("type", tt.ToString())
                .WhereEqual("suggestedPath", cat)
                .WhereNULL("parent")
                .OrderByASC("name")
                .Execute();
            return this.Parse(dt);
        }
        public List<STemplate> GetAllWordExcelTemplates(string cat)
        {
            DataTable dt = this.StartCommand()
                .Select()
                .WhereEqual("suggestedPath", cat)
                .WhereIn("type", new List<string> { "Excel", "Word" })
                .WhereNULL("parent")
                .OrderByASC("name")
                .Execute();
            return this.Parse(dt);
        }
        public List<STemplate> GetAllWordTemplates()
        {
            return this.GetAllByType(TemplateType.Word);
        }
        public List<STemplate> GetAllTextTemplates()
        {
            return this.GetAllByType(TemplateType.Text);
        }
        public List<STemplate> GetAllMailTemplates(string category)
        {
            DataTable dt = this.StartCommand()
                .Select()
                .WhereEqual("suggestedPath", category)
                .WhereIn("type", new List<string> { "Mail" })
                .WhereNULL("parent")
                .OrderByASC("name")
                .Execute();
            return this.Parse(dt);
        }
        private List<STemplate> GetAllByType(TemplateType type)
        {
            DataTable dt = this.StartCommand()
                .Select()
                .WhereEqual("type", type.ToString())
                .OrderByASC("name")
                .Execute();
            List<STemplate> resulting = new();
            return this.Parse(dt);
        }
        public Template GetTemplateByName(string nameOf)
        {
            DataRow dt = this.StartCommand()
                    .Select()
                    .WhereEqual("name", nameOf)
                    .ExecuteOne();
            this.Serialize(dt);
            this.type = (TemplateType)Enum.Parse(typeof(TemplateType), dt["type"].ToString());
            return this;
        }
        public List<STemplate> GetWordTemplates(string category)
        {
            return this.GetAllTemplatesBy(TemplateType.Word, category);
        }
        public List<STemplate> GetExcelTemplates()
        {
            return this.GetAllTemplatesBy(TemplateType.Excel, "");
        }
        public List<string> GetCategories(List<string> types)
        {
            DataTable dt = this.StartCommand()
                .SelectUnique("suggestedPath")
                .WhereIn("type", types)
                .OrderByASC("suggestedPath")
                .Execute();
            List<string> categories = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                categories.Add(dr[0].ToString());
            }
            return categories;
        }

        public void AddTemplate()
        {
            bool isChild = !string.IsNullOrWhiteSpace(this.parent);
            this.StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "name", this.name },
                    { "path", this.path },
                    { "parent", isChild? this.parent.ToString(): Query.Null }, // раньше тут было просто null а теперь будет 'null'
                    { "suggestedPath", isChild? Query.Null : this.suggestedPath },
                    { "type", this.type.ToString() } ,
                    { "settings", this.settings }
                })
                .ExecuteVoid();
            this.id = int.Parse(
                        this.StartCommand()
                            .Select()
                            .WhereEqual("name", this.name)
                            .WhereEqual("path", this.path)
                            .OrderByDESC("id")
                            .ExecuteOne()["id"].ToString()
                );
        }
        public void UpdateTemplate()
        {
            if (this.id == 0)
            {
                this.AddTemplate();
                return;
            }
            this.StartCommand()
                .Update("name", this.name)
                .Update("path", this.path)
                .Update("suggestedPath", this.suggestedPath)
                .Update("settings", this.settings)
                .WhereEqual("id", this.id.ToString())
                .ExecuteVoid();
        }

        public void RemoveTemplate()
        {
            if (ProgramState.IsWorkspaceOpened())
            {
                this.StartCommand()
                    .Delete()
                    .WhereEqual("id", this.id.ToString())
                    .Execute();
            }
        }
        public List<STemplate> GetAllChildren(List<string> ids)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(ids[i]))
                {
                    ids.RemoveAt(i);
                }
            }
            string parents;
            if (ids.Count < 2)
            {
                parents = ids[0];
            }
            else
            {
                parents = string.Join(';', ids);
            }
            DataTable dt = this.StartCommand()
                .Select()
                .WhereEqual("parent", parents)
                .OrderByASC("name")
                .Execute();
            return this.Parse(dt);
        }
        public List<STemplate> GetAllChildren(int id)
        {
            DataTable dt = this.StartCommand()
                .Select()
                .WhereEqual("parent", id)
                .OrderByASC("name")
                .Execute();
            return this.Parse(dt);
        }
        public Template GetChild(string name)
        {
            DataRow dr = this.StartCommand()
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
