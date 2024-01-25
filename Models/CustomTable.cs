using Common;
using Incubator_2.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    public enum TableType
    {
        All,
        OnlyTables,
        OnlyViews
    }
    struct CustomField
    {
        public string name;
        public string view_name;
        public string type;
        public bool not_null;
        public bool is_id;
        public bool is_uniq;
        public string fktable;
        public string fkfield;
    }
    class CustomTable : Model
    {
        public int id { get; set; }
        public string name { get; set; }
        public string viewName { get; set; }
        public List<CustomField> fields { get; set; }

        public CustomTable()
        {
            tableName = "CustomTables";
        }

        public List<string> GetTablesList(TableType type = TableType.OnlyTables)
        {
            string query;
            switch (type)
            {
                case TableType.OnlyTables:
                    query = "type ='table' AND name NOT LIKE 'sqlite_%'";
                    break;
                case TableType.OnlyViews:
                    query = "type ='views' AND name NOT LIKE 'sqlite_%'";
                    break;
                default:
                    query = "name NOT LIKE 'sqlite_%'";
                    break;
            }
            DataTable dt = StartCommandToCustom()
                .AddCustomRequest("SELECT name FROM sqlite_schema WHERE " + query) // SELECT name FROM sqlite_schema WHERE type ='table' AND name NOT LIKE 'sqlite_%'
                .Execute();
            List<string> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["name"].ToString());
            }
            return result;
        }
        public DataTable GetTable(string tableName)
        {
            return StartCommandToCustom().AddCustomRequest($"SELECT * FROM {tableName};").Execute();
        }
        public List<FieldCreator> GetTableFields(string tableName)
        {
            DataTable dt = StartCommandToCustom()
                .AddCustomRequest($"PRAGMA table_info(\"{tableName}\")")
                .Execute();
            List<FieldCreator> creators = new();
            foreach (DataRow dr in dt.Rows)
            {
                FieldCreator fc = new();
                fc.Name = dr["name"].ToString();
                fc.TypeOf = dr["type"].ToString();
                fc.NotNULL = IntToBool((long)dr["notnull"]);
                fc.IsPK = IntToBool((long)dr["pk"]);
                creators.Add(fc);
            }
            return creators;
        }
        public string GetPKField(string tableName)
        {
            DataTable dt = StartCommandToCustom()
                .AddCustomRequest($"PRAGMA table_info(\"{tableName}\")")
                .Execute();
            foreach (DataRow dr in dt.Rows)
            {
                string name = dr["name"].ToString();
                if (IntToBool((long)dr["pk"]))
                {
                    return name;
                }
            }
            return "";
        }

        public List<FieldCreator> GetTableDefinition(string tableName)
        {
            DataTable dt = StartCommandToCustom()
                .AddCustomRequest($"PRAGMA foreign_key_list(\"{tableName}\")")
                .Execute();
            List<FieldCreator> creators = GetTableFields(tableName);
            
            foreach (DataRow dr in dt.Rows)
            {
                string field = dr["from"].ToString();
                string fktable = dr["table"].ToString();
                string fkfield = dr["to"].ToString();
                for (int f = 0; f < creators.Count; f++)
                {
                    if (field == creators[f].Name)
                    {
                        FieldCreator fc = creators[f];
                        fc.FKtable = fktable;
                        fc.FKfield = fkfield;
                        creators[f] = fc;
                    }
                }
            }
            return creators;
        }
        public void InsertInTable(string table, Dictionary<string, string> pairs)
        {
            Query q = StartCommandToCustom();
            q.Table = table;
            q.Insert(pairs);
            q.ExecuteVoid();
        }
        public void DeleteInTable(string table, string field, List<string> values)
        {
            StartCommandToCustom()
                .AddCustomRequest($"DELETE FROM [{table}] WHERE [{field}] IN ('{string.Join("', '", values)}')")
                .ExecuteVoid();
        }
        public DataRow GetOneFromTable(string table, string pk, string pkValue)
        {
            Query q = StartCommandToCustom();
            q.Table = table;
            q.Select();
            q.WhereEqual(pk, pkValue);
            return q.ExecuteOne();
        }
        public void UpdateInTable(string table, string pk, string pkValue, Dictionary<string, string> pairs)
        {
            Query q = StartCommandToCustom();
            q.Table = table;
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                q.Update(pair.Key, pair.Value);
            }
            q.WhereEqual(pk, pkValue);
            q.ExecuteVoid();
        }
    }
}
