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
    public interface IModelConverter
    {
        public Model AsModel();
    }

    public abstract class Model : System.IDisposable
    {
        private bool disposed = false;
        protected string tableName { get; set; }

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
        protected Query StartCommandToService()
        {
            return new Query(this.tableName, DBConnectionType.SERVICE);
        }
        protected Query StartCommandToChat(string path)
        {
            return new Query(this.tableName, path);
        }
        protected Query StartCommandToCustom()
        {
            return new Query(this.tableName, DBConnectionType.CUSTOM);
        }
        

        #region Standart Requests
        protected DataTable GetAll()
        {
            return StartCommand().Select().Execute();
        }
        protected DataRow GetById(int id)
        {
            return StartCommand().Select().WhereEqual("id", id.ToString()).Execute().Rows[0];
        }
        protected DataTable GetByField(string field, string value)
        {
            return StartCommand().Select().WhereEqual(field, value).Execute();
        }
        protected void DeleteById(int id)
        {
            StartCommand().Delete().WhereEqual("id", id.ToString()).ExecuteVoid();
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
        protected bool IntToBool(int value)
        {
            return value == 0? false: true;
        }
        protected bool IntToBool(long value)
        {
            return value == 0 ? false : true;
        }
        protected bool IntToBool(uint value)
        {
            return value == 0 ? false : true;
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
                    property.SetValue(this, Convert.ChangeType(dr[property.Name], property.PropertyType)); // берет из datarow по названию переменной столбец и устанавливает значение к свойству
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        #endregion
        
        private DataRow GetOne(DataTable dt)
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

