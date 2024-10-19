using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading;

namespace Incas.Core.Classes
{
    public sealed class Query
    {
        public const string Null = "!NULL!";
        public string Table;
        public string Result { get; private set; }
        private bool isWhereAlready = false;
        private Dictionary<string, string> parameters = [];
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
        private string Resolve(string paramName)
        {
            return paramName.Replace("-", "_");
        }
        /// <summary>
        /// Return the Guid with @ symbol
        /// </summary>
        /// <param name="value"></param>
        private string RegisterParameter(string value)
        {
            string result = "@" + Guid.NewGuid().ToString("N");
            this.parameters.Add(result, value);
            return result;
        }
        private List<string> RegisterParameters(List<string> values)
        {
            List<string> result = [];
            foreach (string value in values)
            {
                result.Add(this.RegisterParameter(value));
            }
            return result;
        }
        public override string ToString()
        {
            return this.Table;
        }
        public void Clear()
        {
            this.isWhereAlready = false;
            this.Result = "";
        }
        public Query AddCustomRequest(string text)
        {
            this.Result += $"\n{text}";
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
            resulting = $"SELECT {selection}\nFROM [{this.Table}]\n";
            this.Result = resulting;
            return this;
        }

        /// <summary>
        /// SELECT DISTINCT selection FROM Table
        /// </summary>
        /// <returns></returns>
        public Query SelectUnique(string selection = "*")
        {
            this.Select($"DISTINCT {selection}");
            return this;
        }
        /// <summary>
        /// SELECT Count(selection) FROM Table
        /// </summary>
        /// <returns></returns>
        public Query Count(string selection = "*")
        {
            this.Select($"COUNT({selection}) AS count");
            return this;
        }
        #endregion

        #region Limit
        public Query Limit(int limit)
        {
            this.Result += $"\nLIMIT {limit}";
            return this;
        }
        #endregion
        public Query AttachDatabase(string path, string name)
        {
            this.Result = $"ATTACH DATABASE '{path}' AS \"{name}\";\n" + this.Result;
            return this;
        }
        public Query SeparateCommand()
        {
            this.isWhereAlready = false;
            this.Result += ";\n";
            return this;
        }
        private void IfNotExists()
        {
            this.Result += " if not exists";
        }
        private void ReplaceNull()
        {
            this.Result = this.Result.Replace($"'{Null}'", "null");
        }
        #region Transaction
        public Query BeginTransaction()
        {
            this.Result += "BEGIN TRANSACTION;\n";
            return this;
        }
        public Query EndTransaction()
        {
            this.Result += "\nEND TRANSACTION;";
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
            this.Result += $"{start} [{this.Table}] (\"{string.Join("\", \"", dict.Keys)}\")\nVALUES ({string.Join(", ", this.RegisterParameters(dict.Values.ToList()))})";
            this.ReplaceNull();
            return this;
        }
        public Query Update(Dictionary<string, string> dict)
        {
            this.Result += $"UPDATE [{this.Table}]\n" +
            $"SET ";
            List<string> result = [];
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                result.Add("[" + kvp.Key + "] = " + this.RegisterParameter(kvp.Value));
            }
            this.Result += string.Join(", ", result);
            return this;
        }
        public Query Delete()
        {
            this.Result += $"DELETE FROM [{this.Table}]\n";
            return this;
        }
        #endregion

        #region Join
        public Query InnerJoin(string innerTable, string fieldBaseTable, string fieldJoinedTable)
        {
            string resulting;
            resulting = this.Result + $"INNER JOIN [{innerTable}]\nON {this.Table}.{fieldBaseTable}={innerTable}.{fieldJoinedTable}\n";
            this.Result = resulting;
            return this;
        }
        #endregion

        #region Where
        public enum WhereType
        {
            AND,
            OR
        }
        private Query Where(string cell, string comparator, string value, bool isStr, WhereType wt)
        {
            string resulting;
            string c = "";
            if (isStr)
            {
                c = "'";
            }
            if (this.isWhereAlready)
            {
                resulting = this.Result + $"\n{wt} [{cell}] {comparator} {c}{value}{c}\n";
                this.Result = resulting;
                return this;
            }
            else
            {
                this.isWhereAlready = true;
                resulting = this.Result + $"\nWHERE [{cell}] {comparator} {c}{value}{c}\n";
                this.Result = resulting;
                return this;
            }
        }
        /// <summary>
        /// Where A is not NULL
        /// </summary>
        /// <returns></returns>
        public Query WhereNotNULL(string cell, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "is not ", $"NULL AND {cell} = ''", false, wt);
        }
        /// <summary>
        /// Where A is NULL
        /// </summary>
        /// <returns></returns>
        public Query WhereNULL(string cell, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "is ", $"NULL OR {cell} = ''", false, wt);
        }

        /// <summary>
        /// Where A (left arg) = B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereEqual(string cell, string value, bool isStr = true, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "=", value, isStr, wt);
        }
        public Query WhereEqual(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "=", value.ToString(), false, wt);
        }

        /// <summary>
        /// Where A (left arg) != B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereNotEqual(string cell, string value, bool isStr = true, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "<>", value, isStr, wt);
        }
        public Query WhereNotEqual(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "<>", value.ToString(), false, wt);
        }

        /// <summary>
        /// Where A (left arg) < B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereLess(string cell, string value, bool isStr = true, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "<", value, isStr, wt);
        }
        public Query WhereLess(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "<", value.ToString(), false, wt);
        }

        /// <summary>
        /// Where A (left arg) > B (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereMore(string cell, string value, bool isStr = true, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, ">", value, isStr, wt);
        }
        public Query WhereMore(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, ">", value.ToString(), false, wt);
        }

        /// <summary>
        /// Where A (left arg) BETWEEN B (central arg) AND C (right arg)
        /// </summary>
        /// <returns></returns>
        public Query WhereBetween(string cell, int left, int right, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "BETWEEN", $"{left} AND {right}", false, wt);
        }

        /// <summary>
        /// Where A (left arg) IN (args)
        /// </summary>
        /// <returns></returns>
        public Query WhereIn(string cell, List<string> args, WhereType wt = WhereType.AND)
        {
            string resultingString = "('";
            resultingString += string.Join("', '", args);
            resultingString += "')";
            return this.Where(cell, "IN", resultingString, false, wt);
        }
        public Query WhereIn(string cell, List<int> args, WhereType wt = WhereType.AND)
        {
            string resultingString = "(";
            resultingString += string.Join(", ", args);
            resultingString += ")\n";
            return this.Where(cell, "IN", resultingString, false, wt);
        }
        public Query WhereLike(string cell, string value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "LIKE", $"%{value}%", true, wt);
        }
        public Query WhereLike(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "LIKE", $"%{value}%", true, wt);
        }
        public Query WhereNotLike(string cell, string value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "NOT LIKE", $"%{value}%", true, wt);
        }
        public Query WhereNotLike(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "NOT LIKE", $"%{value}%", true, wt);
        }
        public Query WhereStartsWith(string cell, string value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "LIKE", $"{value}%", true, wt);
        }
        public Query WhereStartsWith(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "LIKE", $"{value}%", true, wt);
        }
        public Query WhereNotStartsWith(string cell, string value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "NOT LIKE", $"{value}%", true, wt);
        }
        public Query WhereNotStartsWith(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "NOT LIKE", $"{value}%", true, wt);
        }
        public Query WhereEndsWith(string cell, string value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "LIKE", $"%{value}", true, wt);
        }
        public Query WhereEndsWith(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "LIKE", $"%{value}", true, wt);
        }
        public Query WhereNotEndsWith(string cell, string value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "NOT LIKE", $"%{value}", true, wt);
        }
        public Query WhereNotEndsWith(string cell, int value, WhereType wt = WhereType.AND)
        {
            return this.Where(cell, "NOT LIKE", $"%{value}", true, wt);
        }

        #endregion

        #region Order By
        private Query OrderBy(string columns, string type)
        {
            this.Result += $"\nORDER BY {columns} {type}";
            return this;
        }

        /// <summary>
        /// ORDER BY column ASC
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Query OrderByASC(string column)
        {
            this.OrderBy(column, "ASC");
            return this;
        }

        /// <summary>
        /// ORDER BY column DESC
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Query OrderByDESC(string column)
        {
            this.OrderBy(column, "DESC");
            return this;
        }
        public Query OrderByDateASC(string column)
        {
            this.OrderBy($"date({column})", "ASC");
            return this;
        }
        public Query OrderByDateDESC(string column)
        {
            this.OrderBy($"date({column})", "DESC");
            return this;
        }
        #endregion

        #region Group By
        public Query GroupBy(string column)
        {
            this.Result += $" GROUP BY {column}";
            return this;
        }
        public Query Having(string condition)
        {
            this.Result += $" HAVING {condition}";
            return this;
        }
        #endregion

        public Query CreateTable(string tab, Field[] fields)
        {
            this.Result = $"CREATE TABLE {tab} (\n";
            this.Result += string.Join(",\n", fields.ToString());
            this.Result += ")";
            return this;
        }
        public int GetCount(string column, string table, string condition)
        {
            this.Result = $"SELECT Count([{column}]) AS count FROM [{table}]";
            if (!string.IsNullOrEmpty(condition))
            {
                this.Result += ("WHERE " + condition);
            }
            DataRow dr = this.Execute().Rows[0];
            return int.Parse(dr["count"].ToString());
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
            switch (this.typeOfConnection)
            {
                default:
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
            SQLiteConnection conn = new($"Data source={path}; Version=3; UseUTF16Encoding=True", true);
            return conn;
        }
        private string GetRequest(bool clear = false)
        {
            string tmp = this.Result;
            Console.WriteLine($"[{DateTime.Now}] {tmp}");
            if (clear)
            {
                this.Clear();
            }
            return tmp;
        }
        public Query ShowRequest()
        {
            DialogsManager.ShowInfoDialog(this.Result);
            return this;
        }
        private void ApplyParams(SQLiteCommand cmd)
        {
            foreach (KeyValuePair<string, string> pair in this.parameters)
            {
                cmd.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            this.parameters.Clear();
        }
        public void ShowParameterQuery()
        {
            string result = this.Result;
            foreach (KeyValuePair<string, string> pair in this.parameters)
            {
                result += $"\nPARAM {pair.Key} = {pair.Value}";
            }
            DialogsManager.ShowInfoDialog(result);
        }
        public DataTable Execute()
        {
            try
            {
                using SQLiteConnection conn = this.GetConnection();
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = this.GetRequest();
                this.ApplyParams(cmd);

                SQLiteDataReader sqlreader = cmd.ExecuteReader();
                DataTable objDataTable = new();
                objDataTable.Load(sqlreader);
                conn.Close();
                return objDataTable;

            }
            catch (SQLiteException ex)
            {
                this.SwitchOnSqliteException(ex, ExecuteType.EXECUTE);
                return new DataTable();
            }
        }
        public DataRow ExecuteOne()
        {
            try
            {
                using SQLiteConnection conn = this.GetConnection();
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                System.Diagnostics.Debug.WriteLine(this.DBPath);
                System.Diagnostics.Debug.WriteLine(this.Result);
                cmd.CommandText = this.GetRequest();
                this.ApplyParams(cmd);
                SQLiteDataReader sqlreader = cmd.ExecuteReader();
                DataTable objDataTable = new();
                objDataTable.Load(sqlreader);
                if (objDataTable.Rows.Count > 0)
                {
                    return objDataTable.Rows[0];
                }
                conn.Close();
                return null;

            }
            catch (SQLiteException ex)
            {
                this.SwitchOnSqliteException(ex, ExecuteType.EXECUTE_ONE);
                return null;
            }
        }

        public void ExecuteVoid()
        {
            try
            {
                using SQLiteConnection conn = this.GetConnection();
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = this.GetRequest();
                this.ApplyParams(cmd);
                //this.ShowParameterQuery();
                cmd.ExecuteNonQuery();
                conn.Close();
                return;
            }

            catch (SQLiteException ex)
            {
                this.SwitchOnSqliteException(ex, ExecuteType.EXECUTE_VOID);
            }
            catch (Exception)
            {
                Thread.Sleep(50);
                this.recursion++;
                if (this.recursion < 10)
                {
                    this.ExecuteVoid();
                }
                else
                {
                    this.recursion = 0;
                    DialogsManager.ShowDatabaseErrorDialog("База данных блокируется другим процессом.");
                }
            }
        }
        private void SwitchOnSqliteException(SQLiteException ex, ExecuteType executeType)
        {
            switch (ex.ErrorCode)
            {
                case 1:
                    DialogsManager.ShowDatabaseErrorDialog(
                        $"При выполнении запроса к базе данных возникла ошибка:\n{ex.Message}\nЗапрос: {this.Result}.\nБаза: '{this.DBPath}'" +
                        $"\nINCAS попытается исправить проблему, если она связана с конфигурацией служебной базы данных.");
                    //if (this.typeOfConnection is DBConnectionType.BASE or DBConnectionType.SERVICE)
                    //{
                    //    DatabaseManager.TryFix(ex, this.typeOfConnection);
                    //}
                    break;
                case 5: // busy
                case 6: // locked
                    Thread.Sleep(50);
                    this.recursion++;
                    if (this.recursion < 20)
                    {
                        switch (executeType)
                        {
                            case ExecuteType.EXECUTE:
                                this.Execute();
                                break;
                            case ExecuteType.EXECUTE_VOID:
                                this.ExecuteVoid();
                                break;
                            case ExecuteType.EXECUTE_ONE:
                                this.ExecuteOne();
                                break;
                        }
                    }
                    else
                    {
                        this.recursion = 0;
                        DialogsManager.ShowDatabaseErrorDialog("Не удалось выполнить запрос, поскольку база данных занята другим процессом.");
                    }
                    break;
                case 11:
                case 26:
                    DialogsManager.ShowDatabaseErrorDialog(
                        $"База данных повреждена." +
                        $"\nЕё использование невозможно.\n\nСведения об ошибке: {ex}");
                    break;
                default:
                    DialogsManager.ShowDatabaseErrorDialog(
                        $"При выполнении запроса к базе данных возникла ошибка:\n{ex.Message}" +
                        $"\nПроверьте правильность данных.");
                    break;
            }
        }
        public void Accumulate()
        {
            DatabaseManager.AppendBackgroundQuery(this);
            this.Clear();
        }
        public void Combine(List<Query> queries)
        {
            foreach (Query query in queries)
            {

            }
        }
        #endregion
    }
}
