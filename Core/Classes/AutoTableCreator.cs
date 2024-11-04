using System;
using System.Collections.Generic;
using System.Reflection;

namespace Incas.Core.Classes
{
    public class AutoTableCreator
    {
        private Dictionary<string, FieldCreator> definition = [];
        private string TableName;
        private Type modelClass;
        public AutoTableCreator()
        {
        }
        public AutoTableCreator Initialize(Type type, string name)
        {
            this.definition.Clear();
            this.TableName = name;
            this.modelClass = type;
            this.ParseToDict();
            return this;
        }
        #region Common
        public void ParseToDict()
        {
            foreach (PropertyInfo prop in this.modelClass.GetProperties())
            {
                FieldCreator fc = new(prop.Name, SwitchOnType(prop.PropertyType));
                if (prop.Name is "id" or "Id")
                {
                    fc.IsUNIQUE = true;
                }
                this.definition[prop.Name] = fc;
            }
        }

        public string GetQueryText()
        {
            string result = $"CREATE TABLE IF NOT EXISTS [{this.TableName}] (\n";
            result += string.Join(",\n", this.definition.Values);
            result += "\n);";
            return result;
        }

        public static string SwitchOnType(Type type)
        {
            switch (Type.GetTypeCode(type.GetType()))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return "INTEGER";
                case TypeCode.Double:
                    return "DOUBLE";
                case TypeCode.Boolean:
                    return "BOOLEAN";
                case TypeCode.String:
                case TypeCode.DateTime:
                case TypeCode.Object:
                    return "TEXT";
            }

            return type == typeof(int)
                ? "INTEGER"
                : type == typeof(long) || type == typeof(double)
                    ? "DOUBLE"
                    : type == typeof(DateTime) ? "TEXT" : type == typeof(bool) ? "BOOLEAN" : "STRING";
        }
        #endregion

        public void SetNotNull(string name, bool inNotNull)
        {
            FieldCreator fc = this.definition[name];
            fc.NotNULL = inNotNull;
            this.definition[name] = fc;
        }
        public void SetAsUnique(string name)
        {
            FieldCreator fc = this.definition[name];
            fc.IsUNIQUE = true;
            this.definition[name] = fc;
        }
        public void SetFK(string name, string table, string field)
        {
            FieldCreator fc = this.definition[name];
            fc.FKtable = table;
            fc.FKfield = field;
            this.definition[name] = fc;
        }
        public void SetTextType(string name)
        {
            FieldCreator fc = this.definition[name];
            fc.TypeOf = "TEXT";
            this.definition[name] = fc;
        }
    }
}
