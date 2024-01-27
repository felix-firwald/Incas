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
    class CustomTable : Model
    {
        public CustomTable()
        {

        }

        private Query GetQuery(string tableName, string pathDb)
        {
            Query q = new(tableName);
            q.typeOfConnection = DBConnectionType.CUSTOM;
            q.DBPath = $"{ProgramState.CustomDatabasePath}\\{pathDb}.db";
            //ProgramState.ShowInfoDialog(path);
            return q;
        }

        public List<string> GetTablesList(string pathDb, TableType type = TableType.OnlyTables)
        {
            string query;
            switch (type)
            {
                case TableType.OnlyTables:
                    query = "type ='table' AND name NOT LIKE 'sqlite_%' ORDER BY name ASC";
                    break;
                case TableType.OnlyViews:
                    query = "type ='views' AND name NOT LIKE 'sqlite_%' ORDER BY name ASC";
                    break;
                default:
                    query = "name NOT LIKE 'sqlite_%' ORDER BY name ASC";
                    break;
            }
            DataTable dt = GetQuery(tableName, pathDb)
                .AddCustomRequest("SELECT name FROM sqlite_schema WHERE " + query) // SELECT name FROM sqlite_schema WHERE type ='table' AND name NOT LIKE 'sqlite_%'
                .Execute();
            
            List<string> result = new();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(dr["name"].ToString());
            }
            return result;
        }
        public DataTable GetTable(string table, string pathDb)
        {
            return GetQuery(table, pathDb).AddCustomRequest($"SELECT * FROM [{table}];").Execute();
        }
        public List<FieldCreator> GetTableFields(string table, string pathDb)
        {           
            DataTable dt = GetQuery(tableName, pathDb).AddCustomRequest($"PRAGMA table_info(\"{table}\")")
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
        public string GetPKField(string table, string pathDb)
        {
            DataTable dt = GetQuery(table, pathDb)
                .AddCustomRequest($"PRAGMA table_info(\"{table}\")")
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

        public List<FieldCreator> GetTableDefinition(string table, string pathDb)
        {
            DataTable dt = GetQuery(table, pathDb)
                .AddCustomRequest($"PRAGMA foreign_key_list(\"{table}\")")
                .Execute();
            List<FieldCreator> creators = GetTableFields(table, pathDb);
            
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
        public void InsertInTable(string table, string pathDb, Dictionary<string, string> pairs)
        {
            Query q = GetQuery(table, pathDb);
            q.Table = table;
            q.Insert(pairs);
            q.ExecuteVoid();
        }
        public void DeleteInTable(string table, string field, string pathDb, List<string> values)
        {
            GetQuery(table, pathDb)
                .AddCustomRequest($"DELETE FROM [{table}] WHERE [{field}] IN ('{string.Join("', '", values)}')")
                .ExecuteVoid();
        }
        public DataRow GetOneFromTable(string table, string pk, string pkValue, string pathDb)
        {
            Query q = GetQuery(table, pathDb);
            q.Table = table;
            q.Select();
            q.WhereEqual(pk, pkValue);
            return q.ExecuteOne();
        }
        public void UpdateInTable(string table, string pk, string pkValue, string pathDb, Dictionary<string, string> pairs)
        {
            Query q = GetQuery(table, pathDb);
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
