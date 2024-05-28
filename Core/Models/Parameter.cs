using Common;
using Incas.Core.Classes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Core.Models
{
    public enum ParameterType
    {
        INCUBATOR,
        DATABASE,
        CHAT,
        RESTRICT_EDIT_TABLE,
        MISC
    }
    public class DBParamNotFound : Exception
    {
        public DBParamNotFound(string message) : base(message)
        { }
    }

    internal class Parameter : Model
    {
        public int id { get; private set; }
        public ParameterType type { get; set; }
        public string name { get; set; }
        public string value { get; set; }

        public Parameter()
        {
            this.tableName = "Parameters";
            this.type = ParameterType.MISC;
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
            return this.id > 0;
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
            this.StartCommandToService()
                .Insert(new Dictionary<string, string>
                    {
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
            this.StartCommandToService()
                .Update("value", this.value)
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
    }
}
