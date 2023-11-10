using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Models;
//using Microsoft.Office.Core;

namespace Common
{
    public enum DatabasePermissions
    {
        All,
        AdminOnly,
        ModeratorAndAdmin,
        ModeratorOnly,
        ModeratorAndOperator,
        OperatorOnly
    }
    public enum Types
    {
        INT,
        STR
    }

    public class InvalidIstructionException : Exception 
    {
        public InvalidIstructionException(string message) : base(message) 
        {

        }
    }

    public abstract class Model : System.IDisposable
    {
        private bool disposed = false;
        protected string tableName { get; set; }
        protected string definition { get; set; }

        #region Procedural Context
        private string result;
        private bool isWhereAlready;
        private bool isUpdateAlready;
        #endregion

        #region static 
        public static Dictionary<Type, string> SqliteTypes = new Dictionary<Type, string> 
        {
            { typeof(string), "STRING" },
            { typeof(int), "INT" },
            { typeof(DateTime), "DATETIME" },
            { typeof(bool), "BOOLEAN" },
        };
        #endregion
        public Model()
        {
            Clear();
        }
        public override string ToString()
        {
            return tableName;
        }
        #region Service
        public void Dispose()
        {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }
        private void CleanUp(bool clean)
        {
            if (!this.disposed)
            {
                if (clean)
                {
                    this.disposed = true;
                }
            }
        }
        #endregion
        public void Clear()
        {
            isWhereAlready = false;
            isUpdateAlready = false;
            result = "";
        }

        protected List<string> GetFields()
        {
            List<string> result = new List<string>();
            foreach (var prop in this.GetType().GetProperties())
            {
                result.Add(prop.Name);
                Console.WriteLine(prop.Name, prop.DeclaringType);
            }
            return result;
        }
        protected Query StartCommand()
        {
            return new Query(this.tableName);
        }

        #region Create
        public string CreateDefinition()
        {
            string resulting = $"CREATE TABLE {tableName} (";
            //if (string.IsNullOrEmpty(def))
            //{
            //    throw new InvalidIstructionException($"Empty definition for {tableName} [{MethodBase.GetCurrentMethod().DeclaringType.Name}]");
            //}
            if (definition.Contains("CREATE TABLE"))
            {
                throw new InvalidIstructionException(
                    $"Command 'CREATE TABLE' " +
                    $"can't be in definition of table {tableName}"
                );
            }
            resulting += definition;
            resulting += ");";
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = resulting;
                cmd.ExecuteNonQuery();
            }
            return resulting;
        }
        #endregion


        #region Connection, Request and Permission
        /// <summary>
        /// Возвращает SqlConnection с подставленными значениями, предварительно проверяя доступ
        /// </summary>
        /// <returns></returns>
        private static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data source={ProgramState.DatabasePath}; Version=3; UseUTF16Encoding=True", true);
        }
        private string GetRequest(bool clear=true)
        {
            string tmp = result;
            Console.WriteLine($"[{DateTime.Now}] {tmp}");
            if (clear)
            {
                Clear();
            }
            return tmp;
        }
        private bool CheckPermission(DatabasePermissions permission)
        {
            switch (permission)
            {
                case DatabasePermissions.All: return true;
                case DatabasePermissions.AdminOnly:
                    return Permission.IsUserHavePermission(PermissionGroup.Admin, PermissionMode.Monopoly);
                case DatabasePermissions.ModeratorAndAdmin:
                    return Permission.IsUserHavePermission(PermissionGroup.Moderator, PermissionMode.Rising);
                case DatabasePermissions.ModeratorOnly:
                    return Permission.IsUserHavePermission(PermissionGroup.Moderator, PermissionMode.Monopoly);
                case DatabasePermissions.ModeratorAndOperator:
                    return Permission.IsUserHavePermission(PermissionGroup.Moderator, PermissionMode.Cascade);
                case DatabasePermissions.OperatorOnly:
                    return Permission.IsUserHavePermission(PermissionGroup.Operator, PermissionMode.Monopoly);
                default: throw new Exception($"Unknown permission type \"{permission}\" got.");
            }
        }
        protected int BoolToInt(bool value)
        {
            return value? 1 : 0;
        }
        /// <summary>
        /// Does not support custom types and sets such fields to null
        /// </summary>
        protected void Serialize(DataRow dr)
        {
            foreach (var property in this.GetType().GetProperties())
            {
                try
                {
                    Console.WriteLine($"[{this.GetType()}] {property.Name} = {dr[property.Name]}, ТИП: {property.PropertyType}");
                    property.SetValue(this, Convert.ChangeType(dr[property.Name], property.PropertyType)); // берет из datarow по названию переменной столбец и устанавливает значение к свойству
                }
                catch (Exception ex)
                {
                    property.SetValue(this, null);
                    Console.WriteLine(ex);
                    continue;
                }
            }
            Console.WriteLine("-----------------");
        }
        #endregion
        
        protected DataRow GetOne(DataTable dt)
        {
            try
            {
                return dt.Rows[0];
            }
            catch (Exception)
            {
                return null;
            }
        }
        protected int ParseCount(DataTable dt)
        {
            if (dt.Rows.Count == 0) return 0;
            DataRow dr = GetOne(dt);
            return int.Parse(dr["count"].ToString());
        }

    }
}

