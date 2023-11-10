using Common;
using System;

namespace Models
{
    public class Task : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public bool passed  { get; set; }

        public DateTime dateStarted { get; set; }
        public DateTime dateFinished { get; set; }

        public Task()
        {
            tableName = "Tasks";
            definition = "id INTEGER PRIMARY KEY AUTOINCREMENT,\n" +
                "name STRING NOT NULL,\n" +
                "text TEXT,\n" +
                "passed BOOLEAN NOT NULL DEFAULT (0),\n" +
                "dateStarted TEXT NOT NULL,\n" +
                "dateFinished TEXT NOT NULL,\n" +
                "hidden BOOLEAN NOT NULL DEFAULT (0)";
        }

        public void GetAbsolutelyAllTasks()
        {
            StartCommand()
                .Select()
                .Execute();
        }
        public void GetEnabledTasks()
        {
            StartCommand()
                .Select()
                .WhereMore("dateStarted", DateTime.Now.ToString())
                .Execute();
        }
        public void GetPassedTasks()
        {

        }
    }
}
