using DocumentFormat.OpenXml.VariantTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Common
{
    public enum OnDeleteUpdate
    {
        CASCADE,
        NO_ACTION,
        SET_NULL,
        SET_DEFAULT,
        RESTRICT 
    }
    public enum DBConnectionType
    {
        BASE,
        SERVICE,
        CUSTOM,
        OTHER
    }
    public struct Field
    {
        public string Name;
        public string Type;
        public bool NotNull;
        public string FKTable;
        public string FKField;
        public OnDeleteUpdate Constraint;
        public Field(string name, string type, bool notnull=false, string fkt=null, string fkf="id", OnDeleteUpdate constraint=OnDeleteUpdate.CASCADE)
        {
            Name = name;
            Type = type;
            NotNull = notnull;
            if (fkt != null)
            {
                FKTable = fkt;
                FKField = fkf;
                Constraint = constraint;
            }
            else
            {
                FKTable = "";
                FKField = "";
                Constraint = OnDeleteUpdate.CASCADE;
            }

        }
        private string GetNotNull()
        {
            return NotNull ? " NOT NULL" : "";
        }
        private string GetFK()
        {
            if (!string.IsNullOrEmpty(FKTable))
            {
                return $" REFERENCES {FKTable} ({FKField})";
            }
            return "";
        }
        public override string ToString()
        {
            return $"{Name} {Type}{GetFK()}{GetNotNull()}";
        }
    }
    public sealed class Query
    {
        public readonly string Table;
        public string Result { get; private set; }
        private bool isWhereAlready = false;
        private bool isUpdateAlready = false;
        public DBConnectionType typeOfConnection { get; set; }
        public string DBPath { get; set; }
        public Query(string table, DBConnectionType type = DBConnectionType.BASE) 
        {
            this.Table = table;
            this.typeOfConnection = type;
        }
        public Query(string table, string path)
        {
            this.typeOfConnection = DBConnectionType.OTHER;
            this.Table = table;
            this.DBPath = path;
        }
        public override string ToString()
        {
            return Table;
        }
        public void Clear()
        {
            isWhereAlready = false;
            isUpdateAlready = false;
            Result = "";
        }
        public Query AddCustomRequest(string text)
        {
            Result += $"\n{text}";
            return this;
        }
        #region Select
        /// <summary>
        /// SELECT selection FROM Table
        /// </summary>
        /// <returns></returns>
        public Query Select(string selection = "*")
        {
            string resulting;
            resulting = $"SELECT {selection}\nFROM {Table}\n";
            Result = resulting;
            return this;
        }

        /// <summary>
        /// SELECT DISTINCT selection FROM Table
        /// </summary>
        /// <returns></returns>
        public Query SelectUnique(string selection = "*")
        {
            Select($"DISTINCT {selection}");
            return this;
        }
        /// <summary>
        /// SELECT Count(selection) FROM Table
        /// </summary>
        /// <returns></returns>
        public Query Count(string selection = "*")
        {
            Select($"Count({selection}) AS count");
            return this;
        }
        #endregion

        #region Limit
        public Query Limit(int limit)
        {
            Result += $"\nLIMIT {limit}";
            return this;
        }
        #endregion

        #region Insert Update Delete
        public Query Insert(Dictionary<string, string> dict)
        {
            Result = $"INSERT INTO {Table} ({string.Join(", ", dict.Keys)})\nVALUES ({string.Join(", ", dict.Values)})";
            return this;
        }
        public Query Update(string cell, string value, bool isStr = true)
        {
            string c = "'";
            //if (!isStr) { c = ""; }
            if (isUpdateAlready)
            {
                Result += $",\n" +
                $"{cell} = {c}{value}{c}";
            }
            else
            {
                Result += $"UPDATE {Table}\n" +
                $"SET {cell} = {c}{value}{c}";
                isUpdateAlready = true;
            }
            return this;
        }
        public Query Delete()
        {
            Result += $"DELETE FROM {Table}\n";
            return this;
        }
        #endregion

        #region Join
        public Query InnerJoin(string innerTable, string fieldBaseTable, string fieldJoinedTable)
        {
            string resulting;
            resulting = Result + $"INNER JOIN {innerTable}\nON {Table}.{fieldBaseTable}={innerTable}.{fieldJoinedTable}\n";
            Result = resulting;
            return this;
        }
        #endregion

        #region Where
        private Query Where(string cell, string comparator, string value, bool isStr)
        {
            string resulting;
            string c = "";
            if (isStr)
            {
                c = "'";
            }
            if (isWhereAlready)
            {
                resulting = Result + $"\nAND {cell} {comparator} {c}{value}{c}\n";
                Result = resulting;
                return this;
            }
            else
            {
                isWhereAlready = true;
                resulting = Result + $"\nWHERE {cell} {comparator} {c}{value}{c}\n";
                Result = resulting;
                return this;
            }

        }
        /// <summary>
        /// Where A is not NULL
        /// </summary>
        /// <returns></returns>
        public Query WhereNotNULL(string cell)
        {
            return Where(cell, "is not", "NULL", false);
        }
        public Query WhereNULL(string cell)
        {
            return Where(cell, "is ", "NULL", false);
        }

        /// <summary>
        /// Where A (left arg) = B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereEqual(string cell, string value, bool isStr = true)
        {
            return Where(cell, "=", value, isStr);
        }

        /// <summary>
        /// Where A (left arg) != B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereNotEqual(string cell, string value, bool isStr = true)
        {
            return Where(cell, "<>", value, isStr);
        }

        /// <summary>
        /// Where A (left arg) < B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereLess(string cell, string value, bool isStr = true)
        {
            return Where(cell, "<", value, isStr);
        }

        /// <summary>
        /// Where A (left arg) > B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereMore(string cell, string value, bool isStr = true)
        {
            return Where(cell, ">", value, isStr);
        }

        /// <summary>
        /// Where A (left arg) BETWEEN B (central arg) AND C (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereBetween(string cell, string left, string right, bool isStr = true)
        {
            return Where(cell, "BETWEEN", $"{left} AND {right}", isStr);
        }

        /// <summary>
        /// Where A (left arg) IN (args)
        /// </summary>
        /// <returns></returns>
        public Query WhereIn(string cell, List<string> args)
        {
            string resultingString = "(\"";
            resultingString += string.Join("\", \"", args);
            resultingString += "\")";
            return Where(cell, "IN", resultingString, false);
        }
        public Query WhereIn(string cell, List<int> args)
        {
            string resultingString = "(";
            resultingString += string.Join(", ", args);
            resultingString += ")\n";
            return Where(cell, "IN", resultingString, false);
        }
        public Query WhereLike(string cell, string value)
        {
            return Where(cell, "LIKE", $"%{value}%", true);
        }
        #endregion

        #region Order By
        private Query OrderBy(string columns, string type)
        {
            Result += $"\nORDER BY {columns} {type}";
            return this;
        }

        /// <summary>
        /// ORDER BY column ASC
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Query OrderByASC(string column)
        {
            OrderBy(column, "ASC");
            return this;
        }

        /// <summary>
        /// ORDER BY column DESC
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Query OrderByDESC(string column)
        {
            OrderBy(column, "DESC");
            return this;
        }
        public Query OrderByDateASC(string column)
        {
            OrderBy($"date({column})", "ASC");
            return this;
        }
        public Query OrderByDateDESC(string column)
        {
            OrderBy($"date({column})", "DESC");
            return this;
        }
        #endregion

        #region Group By
        public Query GroupBy(string column)
        {
            Result += $" GROUP BY {column}";
            return this;
        }
        public Query Having(string condition)
        {
            Result += $" HAVING {condition}";
            return this;
        }
        #endregion

        public Query CreateTable(string tab, Field[] fields)
        {
            Result = $"CREATE TABLE {tab} (\n";
            Result += string.Join(",\n", fields.ToString());
            Result += ")";
            return this;
        }

        #region Connection and Request
        private SQLiteConnection GetConnection()
        {
            string path;
            switch (typeOfConnection)
            {
                case DBConnectionType.BASE:
                default:
                    path = ProgramState.DatabasePath;
                    break;
                case DBConnectionType.SERVICE:
                    path = ProgramState.ServiceDatabasePath;
                    break;
                case DBConnectionType.CUSTOM:
                    path = ProgramState.CustomDatabasePath;
                    break;
                case DBConnectionType.OTHER:
                    path = this.DBPath;
                    break;
            }
            SQLiteConnection conn = new SQLiteConnection($"Data source={path}; Version=3; UseUTF16Encoding=True", true);
            return conn;
        }
        private string GetRequest(bool clear = true)
        {
            string tmp = Result;
            Console.WriteLine($"[{DateTime.Now}] {tmp}");
            if (clear)
            {
                Clear();
            }
            return tmp;
        }
        public DataTable Execute()
        {
            try
            {
                using (SQLiteConnection conn = GetConnection())
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = GetRequest();
                    SQLiteDataReader sqlreader = cmd.ExecuteReader();
                    DataTable objDataTable = new DataTable();
                    objDataTable.Load(sqlreader);
                    return objDataTable;
                }

            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog(
                    $"При выполнении запроса к базе данных возникла ошибка:\n{ex}" +
                    $"\nПроверьте правильность данных.",
                    "Ошибка выполнения запроса");
                return new DataTable();
            }
        }
        public DataRow ExecuteOne()
        {
            try
            {
                using (SQLiteConnection conn = GetConnection())
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = GetRequest();
                    SQLiteDataReader sqlreader = cmd.ExecuteReader();
                    DataTable objDataTable = new DataTable();
                    objDataTable.Load(sqlreader);
                    if (objDataTable.Rows.Count > 0)
                    {
                        return objDataTable.Rows[0];
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog(
                    $"При выполнении запроса к базе данных возникла ошибка:\n{ex}" +
                    $"\nПроверьте правильность данных.",
                    "Ошибка выполнения запроса");
                return null;
            }
        }
        public void ExecuteVoid()
        {
            try
            {
                using (SQLiteConnection conn = GetConnection())
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = GetRequest();
                    cmd.ExecuteNonQuery();
                    return;
                }

            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog(
                    $"При выполнении запроса к базе данных возникла ошибка:\n{ex}" +
                    $"\nПроверьте правильность данных.",
                    "Ошибка выполнения запроса");
            }
        }
        #endregion
    }
}
