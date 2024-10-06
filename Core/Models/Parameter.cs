using Incas.Core.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Core.Models
{
    public enum ParameterType
    {
        WORKSPACE,
        DATABASE,
        CONSTANT,
        ENUMERATION,
        USER_CLIPBOARD,
        USER_TASKS,
        MISC
    }
    public class DBParamNotFound : Exception
    {
        public DBParamNotFound(string message) : base(message)
        { }
    }

    internal class Parameter : Model
    {
        public Guid id { get; private set; }
        public ParameterType type { get; set; }
        public string name { get; set; }
        public string value { get; set; }

        public Parameter()
        {
            this.tableName = "Parameters";
            this.type = ParameterType.MISC;
        }
        public void SetValue(object value)
        {
            this.value = JsonConvert.SerializeObject(value);
        }
        public Parameter GetParameter(ParameterType typeOf, string nameOf, string defaultValue = "0", bool createIfNotExists = true)
        {
            DataRow dr = this.StartCommandToService()
                        .Select()
                        .WhereEqual("type", typeOf.ToString())
                        .WhereEqual("name", nameOf)
                        .OrderByASC("id")
                        .ExecuteOne();
            if (dr == null)
            {
                this.type = typeOf;
                this.name = nameOf;
                this.value = defaultValue;
                if (createIfNotExists)
                {
                    this.CreateParameter();
                }
                return this;
            }
            this.Serialize(dr);
            this.type = (ParameterType)Enum.Parse(typeof(ParameterType), dr["type"].ToString());
            return this;
        }
        public Parameter GetParameter(Guid id)
        {
            DataRow dr = this.StartCommandToService()
                        .Select()
                        .WhereEqual("id", id.ToString())
                        .ExecuteOne();
            this.Serialize(dr);
            this.type = (ParameterType)Enum.Parse(typeof(ParameterType), dr["type"].ToString());
            return this;
        }
        public void UpdateParameter(Guid id)
        {
            Dictionary<string, string> dict = new()
            {
                {nameof(this.name), this.name },
                {nameof(this.value), this.value }
            };
            this.StartCommandToService()
                .Update(dict)
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
        }
        public DataTable GetConstants()
        {
            return this.StartCommandToService()
                .Select("[id] AS [Идентификатор], [name] AS [Наименование константы], [value] AS [Значение константы]")
                .WhereEqual("type", ParameterType.CONSTANT.ToString())
                .OrderByASC("[Наименование константы]")
                .Execute();
        }
        public Dictionary<Guid, string> GetConstantsDictionary()
        {
            Dictionary<Guid, string> result = [];
            DataTable dt = this.StartCommandToService()
                .Select("[id], [name]")
                .WhereEqual("type", ParameterType.CONSTANT.ToString())
                .Execute();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(Guid.Parse(dr["id"].ToString()), dr["name"].ToString());
            }
            return result;
        }
        public Dictionary<Guid, string> GetEnumerationsDictionary()
        {
            Dictionary<Guid, string> result = [];
            DataTable dt = this.StartCommandToService()
                .Select("[id], [name]")
                .WhereEqual("type", ParameterType.ENUMERATION.ToString())
                .Execute();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(Guid.Parse(dr["id"].ToString()), dr["name"].ToString());
            }
            return result;
        }
        public List<string> GetConstantsList()
        {
            List<string> list = [];
            DataTable dt = this.GetConstants();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(dr["Наименование константы"].ToString());
                }
            }
            return list;
        }
        public DataTable GetEnumerators()
        {
            return this.StartCommandToService()
                .Select("[id] AS [Идентификатор], [name] AS [Наименование перечисления]")
                .WhereEqual("type", ParameterType.ENUMERATION.ToString())
                .OrderByASC("[Наименование перечисления]")
                .Execute();
        }
        public List<string> GetEnumeratorsList()
        {
            List<string> list = [];
            DataTable dt = this.GetEnumerators();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(dr["Наименование перечисления"].ToString());
                }
            }
            return list;
        }
        public string GetConstantValue(string withName)
        {
            DataRow dr = this.StartCommandToService()
                .Select("value")
                .WhereEqual("type", ParameterType.CONSTANT.ToString())
                .WhereEqual("name", withName)
                .ExecuteOne();
            if (dr is not null)
            {
                return dr["value"].ToString();
            }
            DialogsManager.ShowExclamationDialog($"Константа с именем \"{withName}\" не определена.", "Константа не определена");
            return "";
        }
        public string GetConstantValue(Guid id)
        {
            DataRow dr = this.StartCommandToService()
                .Select("value")
                .WhereEqual("type", ParameterType.CONSTANT.ToString())
                .WhereEqual(nameof(this.id), id.ToString())
                .ExecuteOne();
            return dr is not null ? dr["value"].ToString() : "";
        }
        public List<string> GetEnumerationValue(Guid id)
        {
            DataRow dr = this.StartCommandToService()
                .Select("value")
                .WhereEqual("type", ParameterType.ENUMERATION.ToString())
                .WhereEqual(nameof(this.id), id.ToString())
                .ExecuteOne();
            return dr is not null ? JsonConvert.DeserializeObject<List<string>>(dr["value"].ToString()) : ([]);
        }
        public bool Exists(ParameterType typeOf, string nameOf, string expectedValue, bool like = true)
        {
            Query q = this.StartCommandToService()
                        .Select()
                        .WhereEqual("type", typeOf.ToString())
                        .WhereEqual("name", nameOf);
            if (like)
            {
                q.WhereLike("value", expectedValue);
            }
            else
            {
                q.WhereEqual("value", expectedValue);
            }
            DataRow dr = q.ExecuteOne();
            if (dr == null)
            {
                return false;
            }
            this.Serialize(dr);
            this.type = (ParameterType)Enum.Parse(typeof(ParameterType), dr["type"].ToString());
            return this.id != Guid.Empty;
        }
        public bool GetValueAsBool()
        {
            return !string.IsNullOrEmpty(this.value) && this.value != "0";
        }
        public Parameter WriteBoolValue(bool b)
        {
            this.value = b ? "1" : "0";
            return this;
        }
        public Parameter CreateParameter()
        {
            if (this.id != Guid.Empty)
            {
                return this.UpdateValue();
            }
            this.id = Guid.NewGuid();
            this.StartCommandToService()
                .Insert(new Dictionary<string, string>
                    {
                        {"id", this.id.ToString()},
                        {"type", this.type.ToString()},
                        {"name", this.name},
                        {"value", this.value}
                    }
                )
                .ExecuteVoid();
            return this;
        }
        public Parameter UpdateValue()
        {
            Dictionary<string, string> dict = new()
            {
                {nameof(this.value), this.value }
            };
            this.StartCommandToService()
                .Update(dict)
                .WhereEqual("id", this.id.ToString())
                .ExecuteVoid();
            return this;
        }
        public Parameter DeleteParameterByTypeAndName()
        {
            this.StartCommandToService()
                .Delete()
                .WhereEqual("type", this.type.ToString())
                .WhereEqual("name", this.name)
                .ExecuteVoid();
            return this;
        }
        public void RemoveParameterById(Guid id)
        {
            this.StartCommandToService().Delete().WhereEqual("id", id.ToString()).ExecuteVoid();
        }
    }
}
