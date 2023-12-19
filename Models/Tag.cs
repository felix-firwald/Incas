using Common;
using Incubator_2.Windows;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public enum TypeOfTag
    {
        Variable,
        LocalConstant,
        GlobalEnumeration,
        LocalEnumeration,
    }
    public class Tag : Model
    {
        public int id { get; set; }
        public int template { get; set; }
        public string name { get; set; }
        public TypeOfTag type { get; set; }
        public string value { get; set; }
        public int enumeration { get; set; }
        public int parent { get; set; }
        public bool required { get; set; }
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
                .Execute();
            List<Tag> resulting = new List<Tag>();
            foreach (DataRow dr in dt.Rows)
            {
                Tag mt = new Tag();
                mt.Serialize(dr);
                mt.type = (TypeOfTag)Enum.Parse(typeof(TypeOfTag), dr["type"].ToString());
                resulting.Add(mt);
            }
            return resulting;
        }
        public void AddTag()
        {
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "template", $"'{template}'" },
                    { "name", $"'{name}'" },
                    { "type", $"'{type}'" },
                    { "value", $"'{value}'" },
                    { "enumeration", enumeration.ToString() },
                })
                .ExecuteVoid();
        }
        public void UpdateTag()
        {
            StartCommand()
                .Update("name", name)
                .Update("type", type.ToString())
                .Update("value", value)
                .Update("enumeration", enumeration.ToString())
                .WhereEqual("id", id.ToString())
                .Execute();
        }
        public void RemoveTag()
        {
            StartCommand()
                .Delete()
                .WhereEqual("id", id.ToString())
                .Execute();
        }
        public void RemoveAllTagsByTemplate(int templ)
        {
            StartCommand()
                .Delete()
                .WhereEqual("template", templ.ToString())
                .Execute();
        }
        public void SwitchGlobalToLocalEnumeration(Enumeration enumer)
        {
            StartCommand()
                .Update("value", enumer.content)
                .Update("type", TypeOfTag.LocalEnumeration.ToString())
                .WhereEqual("enumeration", enumer.id.ToString(), false)
                .ExecuteVoid();
        }
    }
}
