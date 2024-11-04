using Incas.Core.Models;
using Incas.Objects.Models;
using Incas.Templates.Models;
using Incas.Users.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Reflection;

namespace Incas.Core.Classes
{
    public static class DatabaseManager // static cause server connection must be only one per session
    {
        private static List<Query> commandsText = [];
        public static SQLiteConnection connection;
        public static void OpenConnection(string path)
        {
            connection = new SQLiteConnection($"Data source={path}; Version=3; datetimeformat=CurrentCulture");
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
        public static void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        public static void InitializePort()
        {
            AutoTableCreator atc = new();
            SQLiteConnection.CreateFile(ServerProcessor.Port);
            Query q = new("")
            {
                typeOfConnection = DBConnectionType.CUSTOM,
                DBPath = ServerProcessor.Port
            };
            q.AddCustomRequest(GetProcessDefinition(atc));
            q.ExecuteVoid();
        }
        public static bool InitializeService(bool createFile = true)
        {
            if (createFile)
            {
                SQLiteConnection.CreateFile(ProgramState.ServiceDatabasePath);
            }           
            AutoTableCreator atc = new();
            Query q = new("")
            {
                typeOfConnection = DBConnectionType.SERVICE
            };
            q
             .AddCustomRequest(GetParameterDefinition(atc))
             .AddCustomRequest(GetUserDefinition(atc))
             .AddCustomRequest(GetGroupDefinition(atc))
             .AddCustomRequest(GetSessionDefinition(atc))
             .AddCustomRequest(GetCommandDefinition(atc))
             .AddCustomRequest(GetClassesDefinition(atc))
             .AddCustomRequest(GetTemplateDefinition(atc))
             .ExecuteVoid();
            return true;
        }

        public static void ActualizeTables()
        {
            if (RegistryData.GetSelectedWorkspace() == "")
            {
                return;
            }
            InitializeService(false);
            CheckFieldsInTable(typeof(Parameter), "Parameters");
            CheckFieldsInTable(typeof(User), "Users");
            CheckFieldsInTable(typeof(Group), "Groups");
            CheckFieldsInTable(typeof(Session), "Sessions");
            CheckFieldsInTable(typeof(Command), "Commands");
            CheckFieldsInTable(typeof(Template), "Templates");
            CheckFieldsInTable(typeof(Class), "Classes");
        }

        private static void CheckFieldsInTable(Type model, string tableName)
        {
            
            Query q = new(tableName, DBConnectionType.SERVICE);
            q.AddCustomRequest($"PRAGMA table_info([{tableName}]);");
            DataTable dt = q.Execute();
            List<string> names = new(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                names.Add((string)row[1]);
            }
            bool needAlter = false;
            string result = $"BEGIN TRANSACTION;\n";
            foreach (PropertyInfo pi in model.GetProperties())
            {
                if (!names.Contains(pi.Name))
                {
                    needAlter = true;
                    result += $"ALTER TABLE [{tableName}] ADD COLUMN {pi.Name} {AutoTableCreator.SwitchOnType(pi.PropertyType)};\n";
                }
            }
            result = result += "COMMIT";
            if (needAlter)
            {
                DialogsManager.ShowDatabaseErrorDialog($"В служебной базе данных не было найдено поле для таблицы {tableName}. Таблица будет обновлена.", "Актуализация базы данных");
                q.Clear();
                q.AddCustomRequest(result + ";");
                q.ExecuteVoid();
            }
        }

        #region Definitions
        private static string GetProcessDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Models.Process), "Processes");
            atc.SetAsUnique("identifier");
            return atc.GetQueryText();
        }
        private static string GetParameterDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Parameter), "Parameters");
            atc.SetNotNull("value", false);
            return atc.GetQueryText();
        }
        private static string GetClassesDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Class), "Classes");
            return atc.GetQueryText();
        }

        private static string GetUserDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(User), "Users");
            atc.SetAsUnique("username");
            atc.SetAsUnique("fullname");
            return atc.GetQueryText();
        }
        private static string GetGroupDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Group), "Groups");
            atc.SetAsUnique("Id");
            return atc.GetQueryText();
        }
        private static string GetSessionDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Session), "Sessions");
            return atc.GetQueryText();
        }
        private static string GetTemplateDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Template), "Templates");
            return atc.GetQueryText();
        }

        private static string GetCommandDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Command), "Commands");
            atc.SetFK("database", "Databases", "path");
            atc.SetNotNull("database", true);
            atc.SetTextType("query");
            return atc.GetQueryText();
        }
        #endregion

        public static void AppendBackgroundQuery(Query q)
        {
            commandsText.Add(q);
        }
        public static async void ExecuteBackground()
        {
            if (commandsText.Count > 0)
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    string result = string.Join(";\n", commandsText);
                    commandsText.Clear();
                    Query q = new("");
                    q.BeginTransaction();
                    q.AddCustomRequest(result + ";");
                    q.EndTransaction();
                    q.ExecuteVoid();
                });
            }
        }
        public static void NullifyBackground()
        {
            commandsText.Clear();
        }
        public static void TryFix(SQLiteException ex, DBConnectionType con)
        {
            if (ex.Message.Contains("no such table:"))
            {
                string[] result = ex.Message.Split();
                AutoTableCreator atc = new();
                Query q = new("")
                {
                    typeOfConnection = con
                };
                switch (result[result.Length - 1].Trim())
                {
                    case "Processes":
                        q.AddCustomRequest(GetProcessDefinition(atc));
                        break;
                    case "Groups":
                        q.AddCustomRequest(GetGroupDefinition(atc));
                        break;
                    case "Parameters":
                        q.AddCustomRequest(GetParameterDefinition(atc));
                        break;
                    case "Users":
                        q.AddCustomRequest(GetParameterDefinition(atc));
                        break;
                    case "Sessions":
                        q.AddCustomRequest(GetSessionDefinition(atc));
                        break;
                    case "Templates":
                        q.AddCustomRequest(GetTemplateDefinition(atc));
                        break;
                    case "Commands":
                        q.AddCustomRequest(GetCommandDefinition(atc));
                        break;
                    case "Classes":
                        q.AddCustomRequest(GetClassesDefinition(atc));
                        break;
                    default:
                        return;
                }
                q.ExecuteVoid();
                DialogsManager.ShowInfoDialog("INCAS исправил ошибку. Программа будет закрыта.");
                ServerProcessor.TerminateProcessHandle();
            }
        }
    }
}

