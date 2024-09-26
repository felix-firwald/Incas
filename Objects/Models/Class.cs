using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Templates.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Objects.Models
{
    public class ClassData
    {
        public List<Field> fields { get; set; }
    }

    public class Class : Model
    {
        public Guid identifier { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string data { get; set; }
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
            List<Class> resulting = new();
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
        public List<Class> GetClassesByCategory(string category)
        {
            return this.FromDataTable(this.StartCommand().Select().WhereEqual(nameof(category), category).Execute());
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
            DataTable dt = this.StartCommandToService().Select().Execute();
            return this.FromDataTable(dt);
        }
        public DataTable GetAllClassesAsDataTable()
        {
            return this.StartCommandToService().Select("[identifier] AS [Идентификатор], [category] AS [Категория], [name] AS [Наименование]").Execute();
        }
        private void Update()
        {
            this.StartCommand()
                .Update(nameof(this.category), this.category)
                .Update(nameof(this.name), this.name)
                .Update(nameof(this.data), this.data)
                .WhereEqual(nameof(this.identifier), this.identifier.ToString())
                .ExecuteVoid();
            ObjectProcessor.UpdateObjectMap(this);
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
                        {nameof(this.data), this.data}
                    }
                ).ExecuteVoid();
            ObjectProcessor.InitializeObjectMap(this);
        }
        public ClassData GetClassData()
        {
            if (string.IsNullOrEmpty(this.data))
            {
                return new();
            }
            return JsonConvert.DeserializeObject<ClassData>(this.data);
        }
        public void SetClassData(ClassData data)
        {
            this.data = JsonConvert.SerializeObject(data);
        }
    }
}
