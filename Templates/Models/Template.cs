using Incas.Core.Classes;
using Incas.Templates.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Incas.Templates.Models
{
    public class Template : Model
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string suggestedPath { get; set; }
        public TemplateType type { get; set; }
        public string parent { get; set; }
        public string settings { get; set; }
        public string fields { get; set; }

        public Template()
        {
            this.tableName = "Templates";
        }
        public Template(Guid newId)
        {
            this.tableName = "Templates";
            this.GetTemplateById(newId);
        }
        public STemplate AsStruct()
        {
            STemplate template = new()
            {
                id = this.id,
                name = this.name,
                path = this.path,
                suggestedPath = this.suggestedPath,
                parent = this.parent,
                type = this.type,
                settings = this.settings,
                fields = this.fields
            };
            return template;
        }

        public List<STemplate> Parse(DataTable dt)
        {
            List<STemplate> resulting = [];
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                this.type = (TemplateType)Enum.Parse(typeof(TemplateType), dr["type"].ToString());
                resulting.Add(this.AsStruct());
            }

            return resulting;
        }

        public Template GetTemplateById(Guid id)
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
                Core.Classes.DialogsManager.ShowErrorDialog("Шаблон с таким идентификатором не был найден.", "Ошибка");
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

        private List<STemplate> GetAllByType(TemplateType type)
        {
            DataTable dt = this.StartCommand()
                .Select()
                .WhereEqual("type", type.ToString())
                .OrderByASC("name")
                .Execute();
            List<STemplate> resulting = [];
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
            List<string> categories = [];
            foreach (DataRow dr in dt.Rows)
            {
                categories.Add(dr[0].ToString());
            }
            return categories;
        }

        public void AddTemplate()
        {
            bool isChild = !string.IsNullOrWhiteSpace(this.parent);
            this.id = Guid.NewGuid();
            this.StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "id", this.id.ToString() },
                    { "name", this.name },
                    { "path", this.path },
                    { "parent", isChild? this.parent.ToString(): null }, // раньше тут было просто null а теперь будет 'null'
                    { "suggestedPath", isChild? "" : this.suggestedPath },
                    { "type", this.type.ToString() },
                    { "settings", this.settings },
                    { "fields", this.fields }
                })
                .ExecuteVoid();
        }
        public void UpdateTemplate()
        {
            if (this.id == Guid.Empty)
            {
                this.AddTemplate();
                return;
            }
            Dictionary<string, string> dict = new()
            {
                { nameof(this.name), this.name},
                { nameof(this.path), this.path},
                { nameof(this.suggestedPath), this.suggestedPath},
                { nameof(this.settings), this.settings },
                { nameof(this.fields), this.fields},
            };

            this.StartCommand()
                .Update(dict)
                .WhereEqual("id", this.id.ToString())
                .ExecuteVoid();
        }

        public void RemoveTemplate()
        {
            this.StartCommand()
                .Delete()
                .WhereEqual("id", this.id.ToString())
                .Execute();
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
            string parents = ids.Count < 2 ? ids[0] : string.Join(';', ids);
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
        public List<Objects.Models.Field> GetFields(bool withoutParent = false)
        {
            List<Objects.Models.Field> tags = new();
            tags = JsonConvert.DeserializeObject<List<Objects.Models.Field>>(this.fields);
            if (!withoutParent)
            {
                if (!string.IsNullOrEmpty(this.parent))
                {
                    DataTable dt = this.StartCommand()
                    .Select("fields")
                    .WhereIn("id", this.parent.Split(';').ToList())
                    .Execute();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tags.AddRange(JsonConvert.DeserializeObject<List<Objects.Models.Field>>(dr["fields"].ToString()));
                    }
                }
            }
            //tags.Sort((x, y) => x.orderNumber.CompareTo(y.orderNumber));
            return tags;
        }
        public void SetTags(List<Objects.Models.Field> tags)
        {
            this.fields = JsonConvert.SerializeObject(tags);
        }
        public void SaveTemplateSettings(TemplateSettings ts)
        {
            this.settings = Cryptographer.EncryptString(JsonConvert.SerializeObject(ts));
        }
    }
}
