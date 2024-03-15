using Common;
using DocumentFormat.OpenXml.Spreadsheet;
using Incubator_2.Common;
using Incubator_2.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Models
{
    public struct TagParameters
    {
        public string Name;
    }
    public struct CommandSettings
    {
        public string Icon;
        public string Name;
        public string Script;
    }
    public enum TypeOfTag
    {
        Variable,
        Text,
        Relation,
        LocalEnumeration,
        LocalConstant,
        Date,
        Generator,
        Table
    }
    public class Tag : Model
    {
        public int id { get; set; }
        public int template { get; set; }
        public string name { get; set; }
        public TypeOfTag type { get; set; }
        public string value { get; set; }
        public int parent { get; set; }
        public string description { get; set; }
        public string command { get; set; }
        public Tag() 
        {
            tableName = "Tags";
        }

        public List<Tag> GetAllTagsByTemplate(int templ, int parent = 0)
        {
            string req = $"SELECT * FROM Tags\nWHERE template = {templ}";
            if (parent != 0)
            {
                req += $" OR template = {parent}";
            }
            DataTable dt = this.StartCommand()
                .AddCustomRequest(req)
                .OrderByASC("id")
                .Execute();
            List<Tag> parentTags = new List<Tag>();
            List<Tag> childrenTags = new List<Tag>();
            foreach (DataRow dr in dt.Rows)
            {
                Tag mt = new Tag();
                mt.Serialize(dr);
                mt.type = (TypeOfTag)Enum.Parse(typeof(TypeOfTag), dr["type"].ToString());
                if (mt.template == parent)
                {
                    parentTags.Add(mt);
                }
                else
                {
                    childrenTags.Add(mt);
                }
            }
            if (parent == 0)    // если это теги НЕунаследованного шаблона
            {
                return childrenTags;
            }
            return ExludeTags(childrenTags, parentTags);
        }
        private List<Tag> ExludeTags(List<Tag> childs, List<Tag> parents)
        {
            foreach (Tag child in childs)   // для каждого тега
            {
                if (child.parent != 0) // если у него есть родитель
                {
                    for (int i = 0; i < parents.Count; i++) // проходим по родителям
                    {
                        if (parents[i].id == child.parent) // если родитель найден
                        {
                            child.name = parents[i].name; // присвоить имя родителя наследнику
                            parents.RemoveAt(i); // удалить родителя (нахуй не нужон)
                            break;
                        }
                    }
                }
            }
            parents.AddRange(childs);
            return parents;
        }
        public void AddTag()
        {
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "template", template.ToString() },
                    { "name", name },
                    { "type", type.ToString() },
                    { "value", value },
                    { "parent", parent > 0? parent.ToString(): Common.Query.Null },
                    { "description", description },
                    { "command", command },
                })
                .ExecuteVoid();
        }
        public void UpdateTag()
        {
            if (id is 0)
            {
                AddTag();
                return;
            }
            StartCommand()
                .Update("name", name)
                .Update("type", type.ToString())
                .Update("value", value)
                .Update("description", description)
                .Update("command", command)
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }
        public void RemoveTag()
        {
            StartCommand()
                .Delete()
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }
        public void RemoveAllTagsByTemplate(int templ)
        {
            StartCommand()
                .Delete()
                .WhereEqual("template", templ.ToString())
                .ExecuteVoid();
        }
        // для работы функции ниже должен быть предварительно обновлен template
        public void GetChild()
        {
            DataRow dr = StartCommand()
                            .Select()
                            .WhereEqual("template", template.ToString())
                            .WhereEqual("parent", id.ToString())
                            .ExecuteOne();
            if (dr != null)
            {
                this.Serialize(dr);
                this.type = (TypeOfTag)Enum.Parse(typeof(TypeOfTag), dr["type"].ToString());
            }           
        }
        public string GetKey()
        {
            return "TAG";
        }
        public CommandSettings GetCommand()
        {
            try
            {
                return JsonConvert.DeserializeObject<CommandSettings>(Cryptographer.DecryptString(this.command));
            }
            catch (Exception)
            {
                return new();
            }
        }
        public void SaveCommand(CommandSettings cs)
        {
            this.command = Cryptographer.EncryptString(JsonConvert.SerializeObject(cs));
        }
    }
}
