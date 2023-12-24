using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Incubator_2.Common
{
    struct FieldCreator
    {
        public string TypeOf;
        public string Name;
        public bool NotNULL = true;
        public bool IsUNIQUE = false;
        public bool IsPK = false;
        public string FKtable = null;
        public string FKfield = null;
        
        public FieldCreator(string nam, string typeOf)
        {
            Name = nam;
            TypeOf = typeOf;
        }
        private string GetNull()
        {
            return NotNULL ? "NOT NULL" : "";
        }
        private string GetFK()
        {
            if (FKtable != null)
            {
                return $"REFERENCES {FKtable} ({FKfield})";
            }
            return "";
        }

        private string GetUniq()
        {
            if (IsUNIQUE)
            {
                return "UNIQUE ON CONFLICT ROLLBACK";
            }
            return "";
        }
        public override string ToString()
        {
            if (IsPK)
            {
                return "id INTEGER PRIMARY KEY AUTOINCREMENT";
            }
            return $"{Name} {TypeOf} {GetNull()} {GetFK()} {GetUniq()}".Trim();
        }
    }
    internal class AutoTableCreator
    {
        private Dictionary<string,FieldCreator> definition = new Dictionary<string, FieldCreator>();
        private string TableName;
        private Type modelClass;
        public AutoTableCreator()
        {
        }
        public AutoTableCreator Initialize(Type type, string name)
        {
            definition.Clear();
            TableName = name;
            modelClass = type;
            ParseToDict();
            return this;
        }
        #region Common
        public void ParseToDict()
        {
            foreach (PropertyInfo prop in modelClass.GetProperties())
            {
                FieldCreator fc = new FieldCreator(prop.Name, SwitchOnType(prop.PropertyType));
                if (prop.Name == "id")
                {
                    fc.IsPK = true;
                }
                definition[prop.Name] = fc;
            }
        }
        

        public string GetQueryText()
        {
            string result = $"CREATE TABLE {TableName} (\n";
            result += string.Join(",\n", definition.Values);
            result += "\n);";
            return result;
        }

        private static string SwitchOnType(Type type)
        {
            if (type == typeof(int))
            {
                return "INTEGER";
            }
            else if (type == typeof(long) || type == typeof(double))
            {
                return "DOUBLE";
            }
            else if (type == typeof(DateTime))
            {
                return "TEXT";
            }
            else if (type == typeof(bool))
            {
                return "BOOLEAN";
            }
            else
            {
                return "STRING";
            }
        }
        #endregion

        public void SetNotNull(string name, bool inNotNull)
        {
            FieldCreator fc = definition[name];
            fc.NotNULL = inNotNull;
            definition[name] = fc;
        }
        public void SetAsUnique(string name)
        {
            FieldCreator fc = definition[name];
            fc.IsUNIQUE = true;
            definition[name] = fc;
        }
        public void SetFK(string name, string table, string field)
        {
            FieldCreator fc = definition[name];
            fc.FKtable = table;
            fc.FKfield = field;
            definition[name] = fc;
        }
    }
}
