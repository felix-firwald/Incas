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
