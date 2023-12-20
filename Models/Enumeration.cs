using Common;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public struct EnumerationView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnumerationView() { }
    }
    public class Enumeration : Model
    {
        public int id;
        public string name;
        public string content;
        public Enumeration() 
        {
            tableName = "Enumerations";
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
                Enumeration en = new Enumeration();
                en.id = int.Parse(dr["id"].ToString());
                en.content = dr["content"].ToString();
                en.name = dr["name"].ToString();
                resulting.Add(en);
            }
            return resulting;
        }
        public Dictionary<int, string> GetAllEnumerationsView()
        {
            DataTable dt = StartCommand()
                                .Select()
                                .OrderByASC("name")
                                .Execute();
            Dictionary<int, string> resulting = new Dictionary<int, string>();
            foreach (DataRow dr in dt.Rows)
            {
                resulting.Add(int.Parse(dr["id"].ToString()), dr["name"].ToString());
            }
            return resulting;
        }

        public Enumeration GetEnumerationByName(string withName)
        {
            DataRow dr = GetOne(StartCommand()
                                    .Select()
                                    .WhereEqual("name", withName)
                                    .Execute());
            this.id = int.Parse(dr["id"].ToString());
            this.content = dr["content"].ToString();
            this.name = dr["name"].ToString();
            return this;
        }
        public Enumeration GetEnumerationById(int pk)
        {
            DataRow dr = GetOne(StartCommand()
                                    .Select()
                                    .WhereEqual("id", pk.ToString())
                                    .Execute());

            this.id = int.Parse(dr["id"].ToString());
            this.content = dr["content"].ToString();
            this.name = dr["name"].ToString();
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
