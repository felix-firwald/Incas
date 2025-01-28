using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Incas.Objects.Models
{
    public class Class : Model
    {
        public Guid identifier { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string data { get; set; }
        private ClassData packedCache { get; set; }
        //public ClassType type { get; set; }
        public Class()
        {
            this.tableName = "Classes";
        }
        public Class(Guid id)
        {
            this.tableName = "Classes";
            this.GetClassById(id);
        }
        private List<Class> FromDataTable(DataTable dt)
        {
            List<Class> resulting = [];
            foreach (DataRow dr in dt.Rows)
            {
                Class c = new();
                c.Serialize(dr);
                resulting.Add(c);
            }
            return resulting;
        }
        public List<string> GetCategories()
        {
            DataTable dt = this.StartCommand()
                .SelectUnique("category")
                .OrderByASC("category")
                .Execute();
            List<string> categories = [];
            foreach (DataRow dr in dt.Rows)
            {
                categories.Add(dr["category"].ToString());
            }
            return categories;

        }
        public List<string> GetCategoriesOfClassType(ClassType type)
        {
            string value = $"\"ClassType\":{(int)type}";
            DataTable dt = this.StartCommand()
                .SelectUnique("category")
                .WhereLike(nameof(this.data), value)
                .OrderByASC("category")
                .Execute();           
            List<string> categories = [];
            foreach (DataRow dr in dt.Rows)
            {
                categories.Add(dr["category"].ToString());
            }
            return categories;

        }
        public List<Class> FindBackReferences(BindingData bd)
        {
            string query = "\"Value\":\"{\\\"Class\\\":\\\"[Class]\\\",\\\"Field\\\":\\\"[Field]\\\"".Replace("[Class]", bd.Class.ToString()).Replace("[Field]", bd.Field.ToString());
            return this.FromDataTable(this.StartCommand().Select().WhereLike(nameof(this.data), query).OrderByASC(nameof(this.name)).Execute());
        }
        public List<Class> FindBackReferences(Guid classId)
        {
            string query = "\"Value\":\"{\\\"Class\\\":\\\"[Class]\\\",".Replace("[Class]", classId.ToString());
            return this.FromDataTable(this.StartCommand().Select().WhereLike(nameof(this.data), query).OrderByASC(nameof(this.name)).Execute());
        }
        public List<string> FindBackReferencesNames(BindingData bd)
        {
            List<string> result = [];
            string query = "\"Value\":\"{\\\"Class\\\":\\\"[Class]\\\",\\\"Field\\\":\\\"[Field]\\\"".Replace("[Class]", bd.Class.ToString()).Replace("[Field]", bd.Field.ToString());
            DataTable dt = this.StartCommand().Select("name").WhereLike(nameof(this.data), query).OrderByASC(nameof(this.name)).Execute();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["name"].ToString());
            }
            return result;
        }
        public List<string> FindBackReferencesNames(Guid classId)
        {
            List<string> result = [];
            string query = "\"Value\":\"{\\\"Class\\\":\\\"[Class]\\\",".Replace("[Class]", classId.ToString());
            DataTable dt = this.StartCommand().Select("name").WhereLike(nameof(this.data), query).OrderByASC(nameof(this.name)).Execute();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["name"].ToString());
            }
            return result;
        }
        private List<Class> GetClassByType(ClassType type)
        {
            string value = $"\"ClassType\":{(int)type}";
            return this.FromDataTable(this.StartCommand().Select().WhereLike(nameof(this.data), value).Execute());
        }
        public List<Class> GetClassesByCategory(string category)
        {
            return this.FromDataTable(this.StartCommand().Select().WhereEqual(nameof(category), category).OrderByASC(nameof(this.name)).Execute());
        }
        public Class GetClassById(Guid id)
        {
            DataRow dr = this.StartCommand()
                .Select()
                .WhereEqual(nameof(this.identifier), id.ToString())
                .ExecuteOne();
            this.Serialize(dr);
            return this;
        }
        public List<Class> GetAllClasses()
        {
            DataTable dt = this.StartCommandToService().Select().OrderByASC("name").Execute();
            return this.FromDataTable(dt);
        }
        public DataTable GetAllClassesGuids()
        {
            return this.StartCommandToService().Select(nameof(this.identifier)).Execute();
        }
        public List<string> GetAllClassesNames()
        {
            DataTable dt = this.StartCommandToService().Select("name").Execute();
            List<string> result = [];
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["name"].ToString());
            }
            return result;
        }
        public DataTable GetAllClassesAsDataTable()
        {
            return this.StartCommandToService().Select("[identifier] AS [Идентификатор], [category] AS [Категория], [name] AS [Наименование]").OrderByASC("Категория ASC, Наименование").Execute();
        }
        private async void Update()
        {          
            Dictionary<string, string> dict = new()
            {
                {
                    nameof(this.category), this.category
                },
                {
                    nameof(this.name), this.name
                },
                {
                    nameof(this.data), this.data
                }
            };
            this.StartCommand()
                .Update(dict)
                .WhereEqual(nameof(this.identifier), this.identifier.ToString())
                .ExecuteVoid();
            await Task.Run(() =>
            {
                ProgramStatusBar.SetText("Обновление полей в карте объектов...");
                Processor.UpdateObjectMap(this);
            });
            ProgramState.UpdateWindowTabs();
        }
        public void Save()
        {
            if (this.identifier == Guid.Empty)
            {
                this.identifier = Guid.NewGuid();
            }
            else
            {
                this.Update();
                return;
            }
            this.StartCommand()
                .Insert(
                new Dictionary<string, string>()
                    {
                        {nameof(this.identifier), this.identifier.ToString()},
                        {nameof(this.category), this.category},
                        {nameof(this.name), this.name},
                        {nameof(this.data), this.data},
                    }
                ).ExecuteVoid();
            Processor.InitializeObjectMap(this);
            ProgramState.UpdateWindowTabs();
        }
        public void Remove(Guid id)
        {
            this.StartCommand().Delete().WhereEqual(nameof(this.identifier), id.ToString()).ExecuteVoid();
            Processor.DropObjectMap(this);
            ProgramState.UpdateWindowTabs();
        }
        /// <summary>
        /// Unpacks class data (fields, properties) if it is not unpacked earlier, otherwise just sends data stored in cache
        /// </summary>
        /// <returns></returns>
        public ClassData GetClassData()
        {
            try
            {
                if (this.packedCache is null)
                {
                    if (this.data is null)
                    {
                        return new();
                    }
                    this.packedCache = JsonConvert.DeserializeObject<ClassData>(this.data);
                }
                return this.packedCache;
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog($"При распаковке класса [{this.name}] возникла ошибка: " + ex.Message);
                return new();
            }
        }
        /// <summary>
        /// Pack class data (fields, etc)
        /// </summary>
        /// <param name="data"></param>
        public void SetClassData(ClassData data)
        {
            this.data = JsonConvert.SerializeObject(data);
        }
    }
}
