using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                return "INTEGER PRIMARY KEY AUTOINCREMENT";
            }
            return $"{Name} {TypeOf} {GetNull()} {GetFK()} {GetUniq()}";
        }
    }
    internal class AutoTableCreator
    {
        private List<FieldCreator> definition = new List<FieldCreator>();
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
                FieldCreator fc = new FieldCreator(prop.Name, SwitchOnType(prop.DeclaringType));
                if (prop.Name == "id")
                {
                    fc.IsPK = true;
                }
                definition.Add(fc);
            }
        }
        

        public string GetQueryText()
        {
            string result = $"CREATE TABLE {TableName} (\n";
            result += string.Join(", ", definition);
            result += "\n);";
            return result;
        }

        private string SwitchOnType(Type type)
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

        private FieldCreator GetField(string name)
        {
            foreach (FieldCreator fc in definition)
            {
                if (fc.Name == name)
                {
                    return fc;
                }
            }
            return new FieldCreator();
        }

        public void SetNotNull(string name, bool inNotNull)
        {
            FieldCreator fc = GetField(name);
            fc.NotNULL = inNotNull;
        }
        public void SetAsUnique(string name)
        {
            FieldCreator fc = GetField(name);
            fc.IsUNIQUE = true;
        }
        public void SetFK(string name, string table, string field)
        {
            FieldCreator fc = GetField(name);
            fc.FKtable = table;
            fc.FKfield = field;
        }
    }
}
