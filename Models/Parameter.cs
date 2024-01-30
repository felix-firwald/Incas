using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Incubator_2.Models
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
    class Parameter : Model
    {
        public int id { get; private set; }
        public ParameterType type { get; set; }
        public string name { get; set; }
        public string value { get; set; }

        public Parameter()
        {
            tableName = "Parameters";
            type = ParameterType.MISC;
        }
        public Parameter GetParameter(ParameterType typeOf, string nameOf, string defaultValue = "0", bool createIfNotExists=true)
        {
            DataRow dr = StartCommandToService()
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
            Query q = StartCommandToService()
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
            return id > 0;
        }
        public bool GetValueAsBool()
        {
            if (string.IsNullOrEmpty(this.value) || this.value == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public Parameter WriteBoolValue(bool b)
        {
            if (b) 
            {
                this.value = "1";
            }
            else
            {
                this.value = "0";
            }
            return this;
        }
        public Parameter CreateParameter()
        {
            StartCommandToService()
                .Insert(new Dictionary<string, string> 
                    {
                        {"type", type.ToString()},
                        {"name", name},
                        {"value", value}
                    }
                )
                .ExecuteVoid();
            return this;
        }
        public Parameter UpdateValue()
        {
            StartCommandToService()
                .Update("value", value)
                .WhereEqual("id", id.ToString())
                .ExecuteVoid();
            return this;
        }
        public Parameter DeleteParameterByTypeAndName()
        {
            StartCommandToService()
                .Delete()
                .WhereEqual("type", type.ToString())
                .WhereEqual("name", name)
                .ExecuteVoid();
            return this;
        }
    }
}
