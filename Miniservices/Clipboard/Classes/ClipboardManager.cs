using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.Miniservices.Clipboard.AutoUI;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Incas.Miniservices.Clipboard.Classes
{
    internal struct ClipboardRecordOld
    {
        public string Name { get; set; }
        public string Text { get; set; }
    }
    internal static class ClipboardManager
    {
        private static List<ClipboardRecord> Cache = [];
        private static Parameter GetByUser()
        {
            using Parameter p = new();
            return p.GetParameter(ParameterType.USER_CLIPBOARD, ProgramState.CurrentWorkspace.CurrentUser.Id.ToString());
        }
        private static void SetByUser(List<ClipboardRecord> records)
        {
            using Parameter p = new();
            p.GetParameter(ParameterType.USER_CLIPBOARD, ProgramState.CurrentWorkspace.CurrentUser.Id.ToString());
            p.Value = JsonConvert.SerializeObject(records);
            p.UpdateValue();
        }
        public static List<ClipboardRecord> GetClipboardRecords()
        {
            try
            {
                List<ClipboardRecord> records = JsonConvert.DeserializeObject<List<ClipboardRecord>>(ClipboardManager.GetByUser().Value);
                ClipboardManager.Cache = records;
                return records;
            }
            catch
            {
                return [];
            }
        }
        public static List<ClipboardRecord> FindByName(string name)
        {
            return ClipboardManager.Cache.FindAll(x => x.Name.ToLower().Contains(name.ToLower()));
        }
        public static void AddToClipboard(ClipboardRecord record)
        {
            List<ClipboardRecord> list = ClipboardManager.GetClipboardRecords();
            list.Add(record);
            list.Sort((x, y) => x.Name.CompareTo(y.Name));
            ClipboardManager.SetByUser(list);
        }
        public static void RemoveFromClipboard(ClipboardRecord record)
        {
            List<ClipboardRecord> list = ClipboardManager.GetClipboardRecords();
            foreach (ClipboardRecord recordToCompare in list)
            {
                if (recordToCompare.Name == record.Name && recordToCompare.Text == record.Text)
                {
                    list.Remove(recordToCompare);
                    break;
                }
            }
            ClipboardManager.SetByUser(list);
        }
        public static void UpdateInClipboard(ClipboardRecordOld oldRecord, ClipboardRecord newRecord)
        {
            List<ClipboardRecord> list = ClipboardManager.GetClipboardRecords();
            foreach (ClipboardRecord record in list)
            {
                if (record.Name == oldRecord.Name && record.Text == oldRecord.Text)
                {
                    record.Name = newRecord.Name;
                    record.Text = newRecord.Text;
                    break;
                }
            }
            ClipboardManager.SetByUser(list);
        }
        public static void ClearCache()
        {
            ClipboardManager.Cache.Clear();
        }
    }
}
