using Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public enum TemplateType
    {
        Word,
        Excel
    }
    public class Template : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string suggestedPath { get; set; }
        public int parent { get; set; }
        public TemplateType type { get; set; }
        public bool isAbstract { get; set; }
        public Template()
        {
            tableName = "Templates";
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
        private List<Template> GetAllTemplatesBy(TemplateType tt, string cat)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("type", tt.ToString())
                .WhereEqual("suggestedPath", cat)
                .WhereNULL("parent")
                .OrderByASC("name")
                .Execute();
            List<Template> resulting = new List<Template>();
            foreach (DataRow dr in dt.Rows)
            {
                Template mt = new Template();
                mt.Serialize(dr);
                mt.type = (TemplateType)Enum.Parse(typeof(TemplateType), dr["type"].ToString());
                resulting.Add(mt);
            }
            return resulting;
        }
        public List<Template> GetAllWordTemplates()
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("type", "Word")
                .OrderByASC("name")
                .Execute();
            List<Template> resulting = new List<Template>();
            foreach (DataRow dr in dt.Rows)
            {
                Template mt = new Template();
                mt.Serialize(dr);
                mt.type = (TemplateType)Enum.Parse(typeof(TemplateType), dr["type"].ToString());
                resulting.Add(mt);
            }
            return resulting;
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
        public List<Template> GetWordTemplates(string category)
        {
            return GetAllTemplatesBy(TemplateType.Word, category);
        }
        public List<Template> GetExcelTemplates()
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
                    { "name", $"'{name}'" },
                    { "path", $"'{path}'" },
                    { "parent", isChild? $"'{parent}'" : "null" },
                    { "suggestedPath", isChild? "''" : $"'{suggestedPath}'" },
                    { "isAbstract", BoolToInt(isAbstract).ToString() },
                    { "type", $"'{type}'" } 
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
        public List<Template> GetAllChildren(List<int> ids)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereIn("parent", ids)
                .OrderByASC("name")
                .Execute();
            List<Template> children = new List<Template>();
            foreach (DataRow dr in dt.Rows)
            {
                Template templ = new Template();
                templ.Serialize(dr);
                children.Add(templ);
            }
            return children;
        }
        public Template GetChild(string name)
        {
            DataRow dr = GetOne(StartCommand()
                .Select()
                .WhereEqual("parent", this.id.ToString())
                .WhereEqual("name", name)
                .Execute());
            this.Serialize(dr);
            return this;

        }
    }
}
