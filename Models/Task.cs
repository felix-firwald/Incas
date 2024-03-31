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
            task.id = id;
            task.name = name;
            task.text = text;
            task.passed = passed;
            task.subtasks = subtasks;
            task.dateFinished = dateFinished;
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
            tableName = "Tasks";
        }
        public STask AsStruct()
        {
            STask result = new();
            result.id = id;
            result.name = name;
            result.text = text;
            result.passed = passed;
            result.subtasks = subtasks;
            result.dateFinished = dateFinished;
            return result;
        }

        public List<STask> GetAllTasks()
        {
            DataTable dt = StartCommand()
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
            DataTable dt = StartCommand()
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
            subtasks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SSubTask>>(input);
        }
        public void SaveTask()
        {
            StartCommand()
                .Insert(new()
                {
                    {nameof(name), name },
                    {nameof(text), text },
                    {nameof(passed), BoolToInt(passed).ToString() },
                    {nameof(dateFinished), dateFinished.ToString() },
                    {nameof(subtasks), ParseSubtasksToJSON()}
                })
                .Execute();
        }
        public void UpdateSubtasks()
        {
            StartCommand()
                .Update(nameof(subtasks), ParseSubtasksToJSON());
        }
    }
}
