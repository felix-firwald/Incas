using Incas.Core.Classes;
using Incas.Miniservices.Tasks.AutoUI;
using IncasEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Incas.Miniservices.Tasks.Classes
{
    public struct TaskboardRecordOld
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Deadline { get; set; }
    }
    public static class TaskManager
    {
        private static List<TaskRecord> Cache = [];
        private static Parameter GetByUser()
        {
            using Parameter p = new();
            return p.GetParameter(ParameterType.USER_TASKS, ProgramState.CurrentWorkspace.CurrentUser.Id.ToString());
        }
        private static void SetByUser(List<TaskRecord> records)
        {
            using Parameter p = new();
            p.GetParameter(ParameterType.USER_TASKS, ProgramState.CurrentWorkspace.CurrentUser.Id.ToString());
            p.Value = JsonConvert.SerializeObject(records);
            p.UpdateValue();
        }
        public static List<TaskRecord> GetTaskboardRecords()
        {
            try
            {
                List<TaskRecord> records = JsonConvert.DeserializeObject<List<TaskRecord>>(TaskManager.GetByUser().Value);
                TaskManager.Cache = records;
                return records;
            }
            catch
            {
                return [];
            }
        }
        public static List<TaskRecord> FindByName(string name)
        {
            return TaskManager.Cache.FindAll(x => x.Name.ToLower().Contains(name.ToLower()));
        }
        public static void AddToTaskboard(TaskRecord record)
        {
            List<TaskRecord> list = TaskManager.GetTaskboardRecords();
            list.Add(record);
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            TaskManager.SetByUser(list);
        }
        public static void RemoveFromTaskboard(TaskRecord record)
        {
            List<TaskRecord> list = TaskManager.GetTaskboardRecords();
            foreach (TaskRecord recordToCompare in list)
            {
                if (recordToCompare.Name == record.Name && recordToCompare.Text == record.Text)
                {
                    list.Remove(recordToCompare);
                    break;
                }
            }
            TaskManager.SetByUser(list);
        }
        public static void UpdateInTaskboard(TaskboardRecordOld oldRecord, TaskRecord newRecord)
        {
            List<TaskRecord> list = TaskManager.GetTaskboardRecords();
            foreach (TaskRecord record in list)
            {
                if (record.Name == oldRecord.Name && record.Text == oldRecord.Text)
                {
                    record.Name = newRecord.Name;
                    record.Text = newRecord.Text;
                    break;
                }
            }
            TaskManager.SetByUser(list);
        }
        public static void ClearCache()
        {
            TaskManager.Cache.Clear();
        }
    }
}
