using Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public class Incubator : Model
    {
        public const int id = 1;
        public string name { get; set; }
        public bool opened { get; set; }
        public bool locked { get; set; }
        public bool repositoryEnabled { get; set; }
        public Incubator() 
        {
            tableName = "Incubator";
            definition = "id INTEGER PRIMARY KEY AUTOINCREMENT,\n" +
                "name STRING NOT NULL,\n" +
                "opened BOOLEAN NOT NULL DEFAULT (True),\n" +
                "locked BOOLEAN NOT NULL DEFAULT (False),\n" +
                "repositoryEnabled BOOLEAN NOT NULL DEFAULT (False)";
        }
        public void InitializeIncubator()
        {
            Random rnd = new Random();
            name = rnd.Next(10001, 19999).ToString();
            opened = true;
            locked = false;
            repositoryEnabled = false;
            StartCommand()
                .Insert(new Dictionary<string, string>
                {
                    { "name", $"'{name}'" },
                    { "opened", opened.ToString() },
                    { "locked", locked.ToString() },
                    { "repositoryEnabled", repositoryEnabled.ToString() },
                })
                .ExecuteVoid();
        }
        public Incubator GetIncubator()
        {

            DataRow dr = GetOne(StartCommand()
                                    .Select()
                                    .Execute());
            this.Serialize(dr);
            return this;
        }
        public void UpdateIncubator()
        {
            StartCommand()
                .Update("name", name)
                .Update("opened", opened.ToString())
                .Update("locked", locked.ToString())
                .Update("repositoryEnabled", locked.ToString())
                .WhereEqual("id", id.ToString())
                .Execute();
        }
        
    }
}
