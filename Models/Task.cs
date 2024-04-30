using Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public struct SSubTask
    {
        public string text { get; set; }
        public bool passed { get; set; }
    }
    public struct STask
    {
        public int id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public bool passed { get; set; }
        public List<SSubTask> subtasks { get; set; }
        public DateTime dateFinished { get; set; }
        public Task AsModel()
        {
            Task task = new();
            task.id = this.id;
            task.name = this.name;
            task.text = this.text;
            task.passed = this.passed;
            task.subtasks = this.subtasks;
            task.dateFinished = this.dateFinished;
            return task;
        }
    }
    public class Task : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public bool passed { get; set; }
        public List<SSubTask> subtasks { get; set; }
        public DateTime dateFinished { get; set; }

        public Task()
        {
            this.tableName = "Tasks";
        }
        public STask AsStruct()
        {
            STask result = new();
            result.id = this.id;
            result.name = this.name;
            result.text = this.text;
            result.passed = this.passed;
            result.subtasks = this.subtasks;
            result.dateFinished = this.dateFinished;
            return result;
        }

        public List<STask> GetAllTasks()
        {
            DataTable dt = this.StartCommand()
                .Select()
                .Execute();
            List<STask> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                result.Add(this.AsStruct());
            }
            return result;
        }
        public List<Task> GetAllTasksModels()
        {
            DataTable dt = this.StartCommand()
                .Select()
                .Execute();
            List<Task> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                this.Serialize(dr);
                result.Add(this);
            }
            return result;
        }

        public void GetPassedTasks()
        {

        }
        private string ParseSubtasksToJSON()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this.subtasks);
        }
        private void ParseSubtasksFromJSON(string input)
        {
            this.subtasks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SSubTask>>(input);
        }
        public void SaveTask()
        {
            this.StartCommand()
                .Insert(new()
                {
                    {nameof(this.name), this.name },
                    {nameof(this.text), this.text },
                    {nameof(this.passed), this.BoolToInt(this.passed).ToString() },
                    {nameof(this.dateFinished), this.dateFinished.ToString() },
                    {nameof(this.subtasks), this.ParseSubtasksToJSON()}
                })
                .Execute();
        }
        public void UpdateSubtasks()
        {
            this.StartCommand()
                .Update(nameof(this.subtasks), this.ParseSubtasksToJSON());
        }
    }
}
