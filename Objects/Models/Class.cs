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
        public string NameTemplate { get; set; }
        public ClassType ClassType { get; set; }
        public bool ShowCard { get; set; }
        public bool EditByAuthorOnly { get; set; }
        public Dictionary<int, StatusData> Statuses { get; set; }
        public Dictionary<int, TemplateData> Templates { get; set; }
        public void AddStatus(StatusData data)
        {
            if (this.Statuses is null)
            {
                this.Statuses = new();
            }
            this.Statuses.Add(this.Statuses.Count + 1, data);
        }
        public void RemoveStatus(int index)
        {
            this.Statuses.Remove(index);
        }
        public void AddTemplate(TemplateData data)
        {
            if (this.Templates is null)
            {
                this.Templates = new();
            }
            this.Templates.Add(this.Templates.Count + 1, data);
        }
        public void EditTemplate(int index, TemplateData data)
        {
            try
            {
                this.Templates[index] = data;
            }
            catch { }
        }
        public void RemoveTemplate(int index)
        {
            this.Templates.Remove(index);
        }
        public List<Field> GetFieldsForMap()
        {
            List<Field> list = new();
            foreach (Field field in this.fields)
            {
                switch (field.Type)
                {
                    case TagType.Variable:
                    case TagType.Text:
                    case TagType.Number:
                    case TagType.Relation:
                    case TagType.LocalEnumeration:
                    case TagType.GlobalEnumeration:
                    case TagType.Date:
                        list.Add(field);
                        break;
                }
            }
            return list;
        }
        public Field FindFieldById(Guid id)
        {
            foreach (Field field in this.fields)
            {
                if (field.Id == id)
                {
                    return field;
                }
            }
            return new();
        }
        public Field FindFieldByName(string name)
        {
            foreach (Field field in this.fields)
            {
                if (field.Name == name)
                {
                    return field;
                }
            }
            return new();
        }
        public Field FindFieldByVisibleName(string name)
        {
            foreach (Field field in this.fields)
            {
                if (field.VisibleName == name)
                {
                    return field;
                }
            }
            return new();
        }
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
            return this.StartCommandToService().Select("[identifier] AS [Идентификатор], [category] AS [Категория], [name] AS [Наименование]").OrderByASC("Категория").Execute();
        }
        private void Update()
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
        public void Remove(Guid id)
        {
            this.StartCommand().Delete().WhereEqual(nameof(this.identifier), id.ToString()).ExecuteVoid();
            ObjectProcessor.DropObjectMap(this);
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
