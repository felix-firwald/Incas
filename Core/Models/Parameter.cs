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
        public Guid Id { get; private set; }
        public ParameterType Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public Parameter()
        {
            this.tableName = "Parameters";
            this.Type = ParameterType.MISC;
        }
        public Parameter(Guid id)
        {
            this.tableName = "Parameters";
            this.Type = ParameterType.MISC;
            this.GetParameter(id);
        }
        public void SetValue(object value)
        {
            this.Value = JsonConvert.SerializeObject(value);
        }
        public Parameter GetParameter(ParameterType typeOf, string nameOf, string defaultValue = "0", bool createIfNotExists = true)
        {
            DataRow dr = this.StartCommandToService()
                        .Select()
                        .WhereEqual(nameof(this.Type), typeOf.ToString())
                        .WhereEqual(nameof(this.Name), nameOf)
                        .Limit(1)
                        .ExecuteOne();
            if (dr == null)
            {
                this.Type = typeOf;
                this.Name = nameOf;
                this.Value = defaultValue;
                if (createIfNotExists)
                {
                    this.CreateParameter();
                }
                return this;
            }
            this.Serialize(dr);
            this.Type = (ParameterType)Enum.Parse(typeof(ParameterType), dr[nameof(this.Type)].ToString());
            return this;
        }
        public Parameter GetParameter(Guid id)
        {
            DataRow dr = this.StartCommandToService()
                        .Select()
                        .WhereEqual(nameof(this.Id), id.ToString())
                        .ExecuteOne();
            this.Serialize(dr);
            this.Type = (ParameterType)Enum.Parse(typeof(ParameterType), dr[nameof(this.Type)].ToString());
            return this;
        }
        public void UpdateParameter(Guid id)
        {
            Dictionary<string, string> dict = new()
            {
                {nameof(this.Name), this.Name },
                {nameof(this.Value), this.Value }
            };
            this.StartCommandToService()
                .Update(dict)
                .WhereEqual(nameof(this.Id), id.ToString())
                .ExecuteVoid();
        }
        public DataTable GetConstants()
        {
            return this.StartCommandToService()
                .Select($"[{nameof(this.Id)}] AS [Идентификатор], [{nameof(this.Name)}] AS [Наименование константы], [{nameof(this.Value)}] AS [Значение константы]")
                .WhereEqual(nameof(this.Type), ParameterType.CONSTANT.ToString())
                .OrderByASC("[Наименование константы]")
                .Execute();
        }
        public Dictionary<Guid, string> GetConstantsDictionary()
        {
            Dictionary<Guid, string> result = [];
            DataTable dt = this.StartCommandToService()
                .Select($"[{nameof(this.Id)}], [{nameof(this.Name)}]")
                .WhereEqual(nameof(this.Type), ParameterType.CONSTANT.ToString())
                .Execute();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(Guid.Parse(dr[nameof(this.Id)].ToString()), dr[nameof(this.Name)].ToString());
            }
            return result;
        }
        public Dictionary<Guid, string> GetEnumerationsDictionary()
        {
            Dictionary<Guid, string> result = [];
            DataTable dt = this.StartCommandToService()
                .Select($"[{nameof(this.Id)}], [{nameof(this.Name)}]")
                .WhereEqual(nameof(this.Type), ParameterType.ENUMERATION.ToString())
                .Execute();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(Guid.Parse(dr[nameof(this.Id)].ToString()), dr[nameof(this.Name)].ToString());
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
                .Select($"[{nameof(this.Id)}] AS [Идентификатор], [{nameof(this.Name)}] AS [Наименование перечисления]")
                .WhereEqual(nameof(this.Type), ParameterType.ENUMERATION.ToString())
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
                .Select(nameof(this.Value))
                .WhereEqual(nameof(this.Type), ParameterType.CONSTANT.ToString())
                .WhereEqual(nameof(this.Name), withName)
                .ExecuteOne();
            if (dr is not null)
            {
                return dr[nameof(this.Value)].ToString();
            }
            DialogsManager.ShowExclamationDialog($"Константа с именем \"{withName}\" не определена.", "Константа не определена");
            return "";
        }
        public string GetConstantValue(Guid id)
        {
            DataRow dr = this.StartCommandToService()
                .Select(nameof(this.Value))
                .WhereEqual(nameof(this.Type), ParameterType.CONSTANT.ToString())
                .WhereEqual(nameof(this.Id), id.ToString())
                .ExecuteOne();
            return dr is not null ? dr[nameof(this.Value)].ToString() : "";
        }
        public List<string> GetEnumerationValue(Guid id)
        {
            DataRow dr = this.StartCommandToService()
                .Select(nameof(this.Value))
                .WhereEqual(nameof(this.Type), ParameterType.ENUMERATION.ToString())
                .WhereEqual(nameof(this.Id), id.ToString())
                .ExecuteOne();
            return dr is not null ? JsonConvert.DeserializeObject<List<string>>(dr[nameof(this.Value)].ToString()) : ([]);
        }
        public bool Exists(ParameterType typeOf, string nameOf, string expectedValue, bool like = true)
        {
            Query q = this.StartCommandToService()
                        .Select()
                        .WhereEqual(nameof(this.Type), typeOf.ToString())
                        .WhereEqual(nameof(this.Name), nameOf);
            if (like)
            {
                q.WhereLike(nameof(this.Value), expectedValue);
            }
            else
            {
                q.WhereEqual(nameof(this.Value), expectedValue);
            }
            DataRow dr = q.ExecuteOne();
            if (dr == null)
            {
                return false;
            }
            this.Serialize(dr);
            this.Type = (ParameterType)Enum.Parse(typeof(ParameterType), dr[nameof(this.Type)].ToString());
            return this.Id != Guid.Empty;
        }
        public bool GetValueAsBool()
        {
            return !string.IsNullOrEmpty(this.Value) && this.Value != "0";
        }
        public Parameter WriteBoolValue(bool b)
        {
            this.Value = b ? "1" : "0";
            return this;
        }
        public Parameter CreateParameter()
        {
            if (this.Id != Guid.Empty)
            {
                return this.UpdateValue();
            }
            this.Id = Guid.NewGuid();
            this.StartCommandToService()
                .Insert(new Dictionary<string, string>
                    {
                        {nameof(this.Id), this.Id.ToString()},
                        {nameof(this.Type), this.Type.ToString()},
                        {nameof(this.Name), this.Name},
                        {nameof(this.Value), this.Value}
                    }
                )
                .ExecuteVoid();
            return this;
        }
        public Parameter UpdateValue()
        {
            Dictionary<string, string> dict = new()
            {
                {nameof(this.Value), this.Value }
            };
            this.StartCommandToService()
                .Update(dict)
                .WhereEqual(nameof(this.Id), this.Id.ToString())
                .ExecuteVoid();
            return this;
        }
        public Parameter DeleteParameterByTypeAndName()
        {
            this.StartCommandToService()
                .Delete()
                .WhereEqual(nameof(this.Type), this.Type.ToString())
                .WhereEqual(nameof(this.Name), this.Name)
                .ExecuteVoid();
            return this;
        }
        public void RemoveParameterById(Guid id)
        {
            this.StartCommandToService().Delete().WhereEqual(nameof(this.Id), id.ToString()).ExecuteVoid();
        }
    }
}
