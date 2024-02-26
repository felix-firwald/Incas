using DocumentFormat.OpenXml.VariantTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
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
    public enum ExecuteType
    {
        EXECUTE,
        EXECUTE_VOID,
        EXECUTE_ONE
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
        public const string Null = "!NULL!";
        public string Table;
        public string Result { get; private set; }
        private bool isWhereAlready = false;
        private bool isUpdateAlready = false;
        private uint recursion = 0;
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
            resulting = $"SELECT {selection}\nFROM [{Table}]\n";
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

        public Query SeparateCommand()
        {
            Result += ";\n";
            return this;
        }
        private void IfNotExists()
        {
            Result += " if not exists";
        }
        private void ReplaceNull()
        {
            Result = Result.Replace($"'{Null}'", "null");
        }
        #region Transaction
        public Query BeginTransaction()
        {
            Result += "BEGIN TRANSACTION;\n";
            return this;
        }
        public Query EndTransaction()
        {
            Result += "\nEND TRANSACTION;";
            return this;
        }
        #endregion
        #region Insert Update Delete
        public Query Insert(Dictionary<string, string> dict, bool replace = false)
        {
            string start = "INSERT INTO";
            if (replace)
            {
                start = "INSERT OR REPLACE INTO";
            }
            Result += $"{start} [{Table}] ([{string.Join("], [", dict.Keys)}])\nVALUES ('{string.Join("', '", dict.Values)}')";
            ReplaceNull();
            return this;
        }
        public Query Update(string cell, string value, bool isStr = true)
        {
            string c = "'";
            //if (!isStr) { c = ""; }
            if (isUpdateAlready)
            {
                Result += $",\n" +
                $"[{cell}] = {c}{value}{c}";
            }
            else
            {
                Result += $"UPDATE [{Table}]\n" +
                $"SET [{cell}] = {c}{value}{c}";
                isUpdateAlready = true;
            }
            return this;
        }
        public Query Delete()
        {
            Result += $"DELETE FROM [{Table}]\n";
            return this;
        }
        #endregion

        #region Join
        public Query InnerJoin(string innerTable, string fieldBaseTable, string fieldJoinedTable)
        {
            string resulting;
            resulting = Result + $"INNER JOIN [{innerTable}]\nON {Table}.{fieldBaseTable}={innerTable}.{fieldJoinedTable}\n";
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
                resulting = Result + $"\nAND [{cell}] {comparator} {c}{value}{c}\n";
                Result = resulting;
                return this;
            }
            else
            {
                isWhereAlready = true;
                resulting = Result + $"\nWHERE [{cell}] {comparator} {c}{value}{c}\n";
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
            return Where(cell, "is not ", $"NULL AND {cell} = ''", false);
        }
        /// <summary>
        /// Where A is NULL
        /// </summary>
        /// <returns></returns>
        public Query WhereNULL(string cell)
        {
            return Where(cell, "is ", $"NULL OR {cell} = ''", false);
        }

        /// <summary>
        /// Where A (left arg) = B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereEqual(string cell, string value, bool isStr = true)
        {
            return Where(cell, "=", value, isStr);
        }
        public Query WhereEqual(string cell, int value)
        {
            return Where(cell, "=", value.ToString(), false);
        }

        /// <summary>
        /// Where A (left arg) != B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereNotEqual(string cell, string value, bool isStr = true)
        {
            return Where(cell, "<>", value, isStr);
        }
        public Query WhereNotEqual(string cell, int value)
        {
            return Where(cell, "<>", value.ToString(), false);
        }

        /// <summary>
        /// Where A (left arg) < B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereLess(string cell, string value, bool isStr = true)
        {
            return Where(cell, "<", value, isStr);
        }
        public Query WhereLess(string cell, int value)
        {
            return Where(cell, "<", value.ToString(), false);
        }

        /// <summary>
        /// Where A (left arg) > B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereMore(string cell, string value, bool isStr = true)
        {
            return Where(cell, ">", value, isStr);
        }
        public Query WhereMore(string cell, int value)
        {
            return Where(cell, ">", value.ToString(), false);
        }

        /// <summary>
        /// Where A (left arg) BETWEEN B (central arg) AND C (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereBetween(string cell, int left, int right)
        {
            return Where(cell, "BETWEEN", $"{left} AND {right}", false);
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
        public Query WhereLike(string cell, int value)
        {
            return Where(cell, "LIKE", $"%{value}%", true);
        }
        public Query WhereNotLike(string cell, string value)
        {
            return Where(cell, "NOT LIKE", $"%{value}%", true);
        }
        public Query WhereNotLike(string cell, int value)
        {
            return Where(cell, "NOT LIKE", $"%{value}%", true);
        }
        public Query WhereStartsWith(string cell, string value)
        {
            return Where(cell, "LIKE", $"{value}%", true);
        }
        public Query WhereStartsWith(string cell, int value)
        {
            return Where(cell, "LIKE", $"{value}%", true);
        }
        public Query WhereNotStartsWith(string cell, string value)
        {
            return Where(cell, "NOT LIKE", $"{value}%", true);
        }
        public Query WhereNotStartsWith(string cell, int value)
        {
            return Where(cell, "NOT LIKE", $"{value}%", true);
        }
        public Query WhereEndsWith(string cell, string value)
        {
            return Where(cell, "LIKE", $"%{value}", true);
        }
        public Query WhereEndsWith(string cell, int value)
        {
            return Where(cell, "LIKE", $"%{value}", true);
        }
        public Query WhereNotEndsWith(string cell, string value)
        {
            return Where(cell, "NOT LIKE", $"%{value}", true);
        }
        public Query WhereNotEndsWith(string cell, int value)
        {
            return Where(cell, "NOT LIKE", $"%{value}", true);
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
        public Query Combine(Query query)
        {
            this.Result += "; " + query.Result.ToString();
            return this;
        }
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
                    path = this.DBPath;
                    break;
                case DBConnectionType.OTHER:
                    path = this.DBPath;
                    break;
            }
            SQLiteConnection conn = new SQLiteConnection($"Data source={path}; Version=3; UseUTF16Encoding=True", true);
            return conn;
        }
        private string GetRequest(bool clear = false)
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
                    System.Diagnostics.Debug.WriteLine(this.DBPath);
                    System.Diagnostics.Debug.WriteLine(Result);
                    cmd.CommandText = GetRequest();
                    
                    SQLiteDataReader sqlreader = cmd.ExecuteReader();
                    DataTable objDataTable = new DataTable();
                    objDataTable.Load(sqlreader);
                    conn.Close();
                    return objDataTable;
                }

            }
            catch (SQLiteException ex)
            {
                SwitchOnSqliteException(ex, ExecuteType.EXECUTE);
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
                    System.Diagnostics.Debug.WriteLine(this.DBPath);
                    System.Diagnostics.Debug.WriteLine(Result);
                    cmd.CommandText = GetRequest();
                    SQLiteDataReader sqlreader = cmd.ExecuteReader();
                    DataTable objDataTable = new DataTable();
                    objDataTable.Load(sqlreader);
                    if (objDataTable.Rows.Count > 0)
                    {
                        return objDataTable.Rows[0];
                    }
                    conn.Close();
                    return null;
                }

            }
            catch (SQLiteException ex)
            {
                SwitchOnSqliteException(ex, ExecuteType.EXECUTE_ONE);
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
                    conn.Close();
                    return;
                }

            }
            
            catch (SQLiteException ex)
            {
                SwitchOnSqliteException(ex, ExecuteType.EXECUTE_VOID);
            }
            catch (Exception)
            {
                Thread.Sleep(50);
                recursion++;
                if (recursion < 10)
                {
                    ExecuteVoid();
                }
                else
                {
                    recursion = 0;
                    ProgramState.ShowDatabaseErrorDialog("База данных блокируется другим процессом.");
                }
            }
        }
        private void SwitchOnSqliteException(SQLiteException ex, ExecuteType executeType)
        {
            switch (ex.ErrorCode)
            {
                case 1:
                    ProgramState.ShowDatabaseErrorDialog(
                        $"При выполнении запроса к базе данных возникла ошибка:\n{ex.Message}" +
                        $"\nINCAS попытается исправить проблему, если она связана с конфигурацией служебной базы данных.");
                    if (typeOfConnection == DBConnectionType.BASE || typeOfConnection == DBConnectionType.SERVICE)
                    {
                        DatabaseManager.TryFix(ex, typeOfConnection);
                    }
                    break;
                case 5: // busy
                case 6: // locked
                    Thread.Sleep(50);
                    recursion++;
                    if (recursion < 20)
                    {
                        switch (executeType)
                        {
                            case ExecuteType.EXECUTE:
                                Execute();
                                break;
                            case ExecuteType.EXECUTE_VOID:
                                ExecuteVoid();
                                break;
                            case ExecuteType.EXECUTE_ONE:
                                ExecuteOne();
                                break;
                        }
                    }
                    else
                    {
                        recursion = 0;
                        ProgramState.ShowDatabaseErrorDialog("Не удалось выполнить запрос, поскольку база данных занята другим процессом.");
                    }
                    break;
                case 11:
                case 26:
                    ProgramState.ShowDatabaseErrorDialog(
                        $"База данных повреждена." +
                        $"\nЕё использование невозможно.\n\nСведения об ошибке: {ex}");
                    break;
                default:

                    ProgramState.ShowDatabaseErrorDialog(
                        $"При выполнении запроса к базе данных возникла ошибка:\n{ex.Message}" +
                        $"\nПроверьте правильность данных.");
                    break;
            }
        }
        public void Accumulate()
        {
            DatabaseManager.AppendBackgroundQuery(this);
            Clear();
        }
        #endregion
    }
}
