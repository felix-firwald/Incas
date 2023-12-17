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
        public bool hidden { get; set; }
        public Template()
        {
            tableName = "Templates";
        }
        private List<Template> GetAllTemplatesBy(TemplateType tt, string cat)
        {
            DataTable dt = StartCommand()
                .Select()
                .WhereEqual("type", tt.ToString())
                .WhereEqual("suggestedPath", cat)
                .WhereNotEqual("hidden", "True")
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
                .WhereNotEqual("hidden", "True")
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
            DataRow dt = GetOne(
                StartCommand()
                    .Select()
                    .WhereEqual("name", nameOf)
                    .Execute()
            );
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
                .WhereEqual("hidden", "False")
                .OrderByASC("suggestedPath")
                .Execute();
            List<string> categories = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                categories.Add(dr[0].ToString());
            }
            return categories;
        }

        public int AddTemplate(bool isChild)
        {
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "name", $"'{name}'" },
                    { "path", $"'{path}'" },
                    { "parent", isChild? $"'{parent}'" : "''" },
                    { "suggestedPath", isChild? "''" : $"'{suggestedPath}'" },
                    { "hidden", isChild? "'True'": "'False'" },
                    { "type", $"'{type}'" }
                })
                .ExecuteVoid();
            return int.Parse(
                    GetOne(
                        StartCommand()
                            .Select()
                            .WhereEqual("name", name)
                            .WhereEqual("path", path)
                            .Execute()
                    )["id"].ToString()
                );
        }
        public void SafetyRemoveTemplate()
        {
            if (ProgramState.CheckWorkspaceOpened())
            {
                StartCommand()
                    .Update("hidden", "True")
                    .WhereEqual("name", name)
                    .Execute();
            }
        }
        public void RemoveTemplate()
        {
            if (ProgramState.CheckWorkspaceOpened())
            {
                StartCommand()
                    .Delete()
                    .WhereEqual("id", id.ToString())
                    .Execute();
            }
        }

    }
}
