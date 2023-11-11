using Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public enum TypeOfTag
    {
        Variable,
        LocalConstant,
        GlobalConstant,
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
        public string constant { get; set; }
        public string enumeration { get; set; }
        public int parent { get; set; }
        public bool required { get; set; }
        public Tag() 
        {
            tableName = "Tags";
            definition = "id INTEGER PRIMARY KEY ASC AUTOINCREMENT,\n" +
                "template INTEGER  REFERENCES Templates (id) ON DELETE CASCADE ON UPDATE CASCADE NOT NULL,\n" +
                "name     STRING  NOT NULL,\n" +
                "type     STRING  NOT NULL,\n" +
                "value    STRING,\n" +
                "constant STRING REFERENCES Constants (name),\n" +
                "enumeration STRING  REFERENCES Enumerations (name) ON DELETE CASCADE ON UPDATE CASCADE";
        }

        public List<Tag> GetAllTagsByTemplate(int templ, int parent = 0)
        {
            string req = $"SELECT * FROM Tags\nWHERE template = {templ}";
            if (parent != 0)
            {
                req += $" OR template = '{parent}'";
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
                    { "constant", $"'{constant}'" },
                    { "enumeration", $"'{enumeration}'" },
                })
                .ExecuteVoid();
        }
        public void UpdateTag()
        {
            StartCommand()
                .Update("name", name)
                .Update("type", type.ToString())
                .Update("value", value)
                .Update("constant", enumeration)
                .Update("enumeration", enumeration)
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
    }
}
