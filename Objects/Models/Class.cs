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
    public class Class : Model, IClass
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public ClassType Type { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
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
        private Class FromDataRow(DataRow row)
        {
            Class c = new();
            c.Type = ParseEnum(row[nameof(c.Type)], ClassType.Model);
            c.Serialize(row);
            return c;
        }
        private List<Class> FromDataTable(DataTable dt)
        {
            List<Class> resulting = [];
            foreach (DataRow dr in dt.Rows)
            {
                resulting.Add(this.FromDataRow(dr));
            }
            return resulting;
        }
        public List<string> GetCategories()
        {
            DataTable dt = this.StartCommand()
                .SelectUnique(nameof(this.Category))
                .OrderByASC(nameof(this.Category))
                .Execute();
            List<string> categories = [];
            foreach (DataRow dr in dt.Rows)
            {
                categories.Add(dr[nameof(this.Category)].ToString());
            }
            return categories;

        }
        public List<string> GetCategoriesOfClassType(ClassType type)
        {
            string value = $"\"ClassType\":{(int)type}";
            DataTable dt = this.StartCommand()
                .SelectUnique(nameof(this.Category))
                .WhereLike(nameof(this.Data), value)
                .OrderByASC(nameof(this.Category))
                .Execute();           
            List<string> categories = [];
            foreach (DataRow dr in dt.Rows)
            {
                categories.Add(dr[nameof(this.Category)].ToString());
            }
            return categories;

        }
        public List<Class> FindBackReferences(BindingData bd)
        {
            string query = "\"Value\":\"{\\\"Class\\\":\\\"[Class]\\\",\\\"Field\\\":\\\"[Field]\\\"".Replace("[Class]", bd.Class.ToString()).Replace("[Field]", bd.Field.ToString());
            return this.FromDataTable(this.StartCommand().Select().WhereLike(nameof(this.Data), query).OrderByASC(nameof(this.Name)).Execute());
        }
        public List<Class> FindBackReferences(Guid classId)
        {
            string query = "\"Value\":\"{\\\"Class\\\":\\\"[Class]\\\",".Replace("[Class]", classId.ToString());
            return this.FromDataTable(this.StartCommand().Select().WhereLike(nameof(this.Data), query).OrderByASC(nameof(this.Name)).Execute());
        }
        public List<string> FindBackReferencesNames(BindingData bd)
        {
            List<string> result = [];
            string query = "\"Value\":\"{\\\"Class\\\":\\\"[Class]\\\",\\\"Field\\\":\\\"[Field]\\\"".Replace("[Class]", bd.Class.ToString()).Replace("[Field]", bd.Field.ToString());
            DataTable dt = this.StartCommand().Select("name").WhereLike(nameof(this.Data), query).OrderByASC(nameof(this.Name)).Execute();
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
            DataTable dt = this.StartCommand().Select("name").WhereLike(nameof(this.Data), query).OrderByASC(nameof(this.Name)).Execute();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["name"].ToString());
            }
            return result;
        }
        private List<Class> GetClassByType(ClassType type)
        {
            string value = $"\"ClassType\":{(int)type}";
            return this.FromDataTable(this.StartCommand().Select().WhereLike(nameof(this.Data), value).Execute());
        }
        public List<Class> GetClassesByCategory(string category)
        {
            return this.FromDataTable(this.StartCommand().Select().WhereEqual(nameof(this.Category), category).OrderByASC(nameof(this.Name)).Execute());
        }
        public Class GetClassById(Guid id)
        {
            DataRow dr = this.StartCommand()
                .Select()
                .WhereEqual(nameof(this.Id), id.ToString())
                .ExecuteOne();
            this.FromDataRow(dr);
            return this;
        }
        public List<Class> GetAllClasses()
        {
            DataTable dt = this.StartCommandToService().Select().OrderByASC(nameof(this.Name)).Execute();
            return this.FromDataTable(dt);
        }
        public DataTable GetAllClassesGuids()
        {
            return this.StartCommandToService().Select(nameof(this.Id)).Execute();
        }
        public List<string> GetAllClassesNames()
        {
            DataTable dt = this.StartCommandToService().Select(nameof(this.Name)).Execute();
            List<string> result = [];
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr[nameof(this.Name)].ToString());
            }
            return result;
        }
        public DataTable GetAllClassesAsDataTable()
        {
            return this.StartCommandToService().Select($"[{nameof(this.Id)}] AS [Идентификатор], [{nameof(this.Category)}] AS [Категория], [{nameof(this.Name)}] AS [Наименование]").OrderByASC("Категория ASC, Наименование").Execute();
        }
        private async void Update()
        {          
            Dictionary<string, string> dict = new()
            {
                {
                    nameof(this.Category), this.Category
                },
                {
                    nameof(this.Name), this.Name
                },
                {
                    nameof(this.Data), this.Data
                }
            };
            this.StartCommand()
                .Update(dict)
                .WhereEqual(nameof(this.Id), this.Id.ToString())
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
            if (this.Id == Guid.Empty)
            {
                this.Id = Guid.NewGuid();
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
                        {nameof(this.Id), this.Id.ToString()},
                        {nameof(this.Category), this.Category},
                        {nameof(this.Name), this.Name},
                        {nameof(this.Data), this.Data},
                    }
                ).ExecuteVoid();
            Processor.InitializeObjectMap(this);
            ProgramState.UpdateWindowTabs();
        }
        public void Remove(Guid id)
        {
            this.StartCommand().Delete().WhereEqual(nameof(this.Id), id.ToString()).ExecuteVoid();
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
                    if (this.Data is null)
                    {
                        return new();
                    }
                    this.packedCache = JsonConvert.DeserializeObject<ClassData>(this.Data);
                }
                return this.packedCache;
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog($"При распаковке класса [{this.Name}] возникла ошибка: " + ex.Message);
                return new();
            }
        }
        /// <summary>
        /// Pack class data (fields, etc)
        /// </summary>
        /// <param name="data"></param>
        public void SetClassData(ClassData data)
        {
            this.Data = JsonConvert.SerializeObject(data);
        }
    }
}
