using Common;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public class Enumeration : Model
    {
        public string name;
        public string content;
        public Enumeration() 
        {
            tableName = "Enumerations";
            definition = "name STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL,\n" +
                "content STRING NOT NULL,\n" +
                "hidden BOOLEAN NOT NULL DEFAULT (False)";
        }
        public string[] GetValues()
        {
            return content.Split('\n');
        }
        public List<Enumeration> GetAllEnumerations() 
        {
            DataTable dt = StartCommand()
                                .Select()
                                .OrderByASC("name")
                                .Execute();
            List<Enumeration> resulting = new List<Enumeration>();
            foreach (DataRow dr in dt.Rows)
            {
                //Console.WriteLine(dr["name"]);
                //Console.WriteLine(dr["content"]);
                Enumeration en = new Enumeration();
                en.name = dr["name"].ToString();
                en.content = dr["content"].ToString();
                // en.Serialize(dr);
                
                resulting.Add(en);
            }
            return resulting;
        }

        public Enumeration GetEnumerationByName()
        {
            DataRow dt = GetOne(StartCommand()
                                    .Select()
                                    .WhereEqual("name", name)
                                    .Execute());
            this.content = dt["content"].ToString();
            return this;
        }
        public void AddEnumeration()
        {
            StartCommand()
                .Insert(new Dictionary<string, string>
                    {
                        { "name", $"'{name}'" },
                        { "content", $"'{content}'" }
                    })
                .ExecuteVoid();
        }
        public void UpdateEnumeration()
        {
            StartCommand()
                .Update("content", content)
                .WhereEqual("name", this.name)
                .ExecuteVoid();
        }
        public void RemoveEnumeration()
        {
            StartCommand()
                .Delete()
                .WhereEqual("name", this.name)
                .Execute();
        }
    }
}
