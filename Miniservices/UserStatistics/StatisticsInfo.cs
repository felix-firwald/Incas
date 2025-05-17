using ABI.System;
using Incas.Core.Classes;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Miniservices.UserStatistics
{
    public class StatisticsInfo
    {
        public enum DefaultMenuView
        {
            Standart,
            Collapsed
        }
        private enum ClassOperationType
        {
            Read,
            Search,
            Write
        }
        public class ClassInfo
        {
            public Guid Class { get; set; }
            public int CommonInteractionsCount { get; set; }
            public byte WritingOperationsCount { get; set; }
            public byte ReadingOperationsCount { get; set; }
            public byte SearchingOperationsCount { get; set; }
            public Guid DefaultSearchField { get; set; }
            public Dictionary<Guid, object> GroupFiltersLast { get; set; }
            public DateTime LastClassInteraction { get; set; }
            public void AddClassInteractionRead()
            {
                this.CommonInteractionsCount++;
                this.ReadingOperationsCount++;
                this.ReadingOperationsCount = this.ReadingOperationsCount.Clamp(0, 10);
                this.LastClassInteraction = DateTime.Now;
            }
            public void AddClassInteractionWrite()
            {
                this.CommonInteractionsCount++;
                this.WritingOperationsCount++;
                this.WritingOperationsCount = this.WritingOperationsCount.Clamp(0, 10);
                this.LastClassInteraction = DateTime.Now;
            }
            public void AddClassInteractionSearch(Guid search)
            {
                this.CommonInteractionsCount++;
                this.SearchingOperationsCount++;
                this.SearchingOperationsCount = this.SearchingOperationsCount.Clamp(0, 10);
                this.DefaultSearchField = search;
                this.LastClassInteraction = DateTime.Now;
            }
            public void Clamp(int maxCommon)
            {
                this.CommonInteractionsCount = this.CommonInteractionsCount.Clamp(0, maxCommon);
            }
        }
        public struct ObjectInfo
        {
            public Guid Class { get; set; }
            public Guid Object { get; set; }
            public string Name { get; set; }
            public DateTime DateTime { get; set; }
        }
        public Dictionary<Guid, ClassInfo> Classes { get; set; }
        public List<ObjectInfo> PreviouslyObjectsWorked { get; set; }
        public DefaultMenuView DefaultMenuViewMode { get; set; }
        public delegate void ObjectsInfoUpdated();
        public event ObjectsInfoUpdated OnObjectsInfoUpdated;

        public StatisticsInfo()
        {
            this.Classes = new();
            this.PreviouslyObjectsWorked = new();
        }
        public async void AddClassInteractionRead(IClass cl)
        {
            await Task.Run(() =>
            {
                if (this.Classes.ContainsKey(cl.Id))
                {
                    this.Classes[cl.Id].AddClassInteractionRead();
                    return;
                }
                else
                {
                    ClassInfo ci = new()
                    {
                        Class = cl.Id
                    };
                    ci.AddClassInteractionRead();
                    this.Classes.Add(cl.Id, ci);
                }                
            });
        }
        public async void AddClassInteractionWrite(IClass cl)
        {
            await Task.Run(() =>
            {
                if (this.Classes.ContainsKey(cl.Id))
                {
                    this.Classes[cl.Id].AddClassInteractionRead();
                    return;
                }
                else
                {
                    ClassInfo ci = new()
                    {
                        Class = cl.Id
                    };
                    ci.AddClassInteractionWrite();
                    this.Classes.Add(cl.Id, ci);
                }
            });
        }
        public async void AddClassInteractionSearch(IClass cl, Field searchField)
        {
            await Task.Run(() =>
            {
                if (this.Classes.ContainsKey(cl.Id))
                {
                    this.Classes[cl.Id].AddClassInteractionRead();
                    return;
                }
                else
                {
                    ClassInfo ci = new()
                    {
                        Class = cl.Id
                    };
                    ci.AddClassInteractionSearch(searchField.Id);
                    this.Classes.Add(cl.Id, ci);
                }
            });
        }
        public Guid GetSearchTargetFor(IClass cl)
        {
            if (this.Classes.ContainsKey(cl.Id))
            {
                return this.Classes[cl.Id].DefaultSearchField;
            }
            return Guid.Empty;
        }
        public string GetFilterValueFor(IClass cl, Field f)
        {
            try
            {
                if (this.Classes.ContainsKey(cl.Id))
                {
                    ClassInfo ci = this.Classes[cl.Id];
                    if (ci.GroupFiltersLast is null)
                    {
                        ci.GroupFiltersLast = new();
                        return "";
                    }
                    if (ci.GroupFiltersLast.ContainsKey(f.Id))
                    {
                        return ci.GroupFiltersLast[f.Id].ToString();
                    }                   
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        public void SetFilterValueFor(IClass cl, Field f, string value)
        {
            try
            {
                if (this.Classes.ContainsKey(cl.Id))
                {
                    this.Classes[cl.Id].GroupFiltersLast[f.Id] = value;
                }
                else
                {
                    ClassInfo ci = new()
                    {
                        Class = cl.Id
                    };
                    ci.GroupFiltersLast[f.Id] = value;
                    this.Classes.Add(cl.Id, ci);
                }
            }
            catch
            {
                
            }
        }
        private static string GetFileName()
        {
            return $"{WorkspacePaths.UserPathWorkspaceCache}\\statistics.json";
        }
        public static StatisticsInfo Load()
        {
            StatisticsInfo info = new();
            try
            {
                info = JsonConvert.DeserializeObject<StatisticsInfo>(File.ReadAllText(GetFileName()));
                if (info == null)
                {
                    return new();
                }

                foreach (KeyValuePair<Guid, ClassInfo> classInfo in info.Classes)
                {
                    if (classInfo.Value.GroupFiltersLast is null)
                    {
                        classInfo.Value.GroupFiltersLast = new();
                    }
                    classInfo.Value.Clamp(15);
                }
            }
            catch (System.Exception)
            {
                //DialogsManager.ShowErrorDialog(ex);
            }
            return info;
        }
        public void Save()
        {
            foreach (KeyValuePair<Guid, ClassInfo> classInfo in this.Classes)
            {
                classInfo.Value.Clamp(15);
            }
            string result = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(GetFileName(), result);         
        }
        public async void AddObjects(List<IObject> objs)
        {
            await Task.Run(() =>
            {
                List<ObjectInfo> list = new();
                foreach (IObject obj in objs)
                {
                    ObjectInfo info = new()
                    {
                        Object = obj.Id,
                        Class = obj.Class.Id,
                        DateTime = DateTime.Now,
                        Name = obj.Name
                    };
                    list.Add(info);
                }
                
                this.PreviouslyObjectsWorked.InsertRange(0, list);
                this.PreviouslyObjectsWorked = this.PreviouslyObjectsWorked.Take(20).ToList();
                this.OnObjectsInfoUpdated?.Invoke();
            });
        }
    }
}
