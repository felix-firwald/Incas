using Incubator_2.Common;
using Incubator_2.Models;
using Incubator_2.Properties;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

using System.Data.SQLite;
using System.Windows;
using System.Windows.Markup;

namespace Common
{
    public static class DatabaseManager // static cause server connection must be only one per session
    {
        private static List<string> commandsText = new();
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
            AutoTableCreator atc = new AutoTableCreator();
            SQLiteConnection.CreateFile(ServerProcessor.Port);
            Query q = new Query("");
            q.typeOfConnection = DBConnectionType.CUSTOM;
            q.DBPath = ServerProcessor.Port;
            q.AddCustomRequest(GetProcessDefinition(atc));
            q.ExecuteVoid();
        }
        public static void InitializeData(string customName = null)
        {
            AutoTableCreator atc = new AutoTableCreator();
            Query q = new Query("");
            if (customName == null)
            {
                q.typeOfConnection = DBConnectionType.BASE;
            }
            else
            {
                customName = $"{ProgramState.CommonPath}\\{customName}.dbinc";
                SQLiteConnection.CreateFile(customName);
                q.typeOfConnection = DBConnectionType.OTHER;
                q.DBPath = customName;
            }
            q.AddCustomRequest(GetTaskDefinition(atc))
             .AddCustomRequest(GetSubtaskDefinition(atc))
             .AddCustomRequest(GetTemplateDefinition(atc))
             .AddCustomRequest(GetTagDefinition(atc))
             .AddCustomRequest(GetGeneratedDocumentDefinition(atc))
             .ExecuteVoid();
            //SQLiteConnection.ClearAllPools();
        }

        public static bool InitializeService()
        {
            SQLiteConnection.CreateFile(ProgramState.DatabasePath);
            SQLiteConnection.CreateFile(ProgramState.ServiceDatabasePath);
            AutoTableCreator atc = new AutoTableCreator();
            Query q = new Query("");
            q.typeOfConnection = DBConnectionType.SERVICE;
            q
             .AddCustomRequest(GetParameterDefinition(atc))
             .AddCustomRequest(GetSectorDefinition(atc))
             .AddCustomRequest(GetUserDefinition(atc))
             .AddCustomRequest(GetSessionDefinition(atc))
             .AddCustomRequest(GetDatabasesDefinition(atc))
             .AddCustomRequest(GetCommandDefinition(atc))
             
             .ExecuteVoid();
            InitializeData();
            return true;
        }
        public static void CreateChatDatabase(string chat, int user1, int user2)
        {
            using (Parameter p = new())
            {
                p.name = $"{user1}-{user2}";
                p.type = ParameterType.CHAT;
                p.value = chat;
                p.CreateParameter();
            }
            string db = ProgramState.Messages + $"\\{chat}.dbinc";
            SQLiteConnection.CreateFile(db);
            AutoTableCreator atc = new AutoTableCreator();
            Query q = new Query("");
            q.typeOfConnection = DBConnectionType.OTHER;
            q.DBPath = db;
            q.AddCustomRequest(GetMessageDefinition(atc))
             .ExecuteVoid();
        }

        #region Definitions
        private static string GetProcessDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Incubator_2.Models.Process), "Processes");
            atc.SetAsUnique("identifier");
            return atc.GetQueryText();
        }
        private static string GetSectorDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Sector), "Sectors");
            atc.SetAsUnique("slug");
            atc.SetAsUnique("name");
            return atc.GetQueryText();
        }
        private static string GetDatabasesDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Database), "Databases");
            atc.SetAsUnique("name");
            atc.SetAsUnique("path");
            atc.SetNotNull("name", true);
            atc.SetNotNull("path", true);
            return atc.GetQueryText();
        }

        private static string GetMessageDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Message), "Messages");
            return atc.GetQueryText();
        }

        private static string GetParameterDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Parameter), "Parameters");
            atc.SetNotNull("value", false);
            return atc.GetQueryText();
        }

        private static string GetUserDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(User), "Users");
            atc.SetAsUnique("username");
            atc.SetAsUnique("fullname");
            return atc.GetQueryText();
        }
        private static string GetSessionDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Session), "Sessions");
            return atc.GetQueryText();
        }
        private static string GetTaskDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Task), "Tasks");
            return atc.GetQueryText();
        }
        private static string GetSubtaskDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Subtask), "Subtasks");
            atc.SetFK("task", "Tasks", "id");
            return atc.GetQueryText();
        }
        private static string GetTemplateDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Template), "Templates");
            atc.SetFK("parent", "Templates", "id");
            return atc.GetQueryText();
        }
        private static string GetGeneratedDocumentDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(GeneratedDocument), "GeneratedDocuments");
            atc.SetFK("template", "Templates", "id");
            atc.SetAsUnique("reference");
            return atc.GetQueryText();
        }
        
        private static string GetTagDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Tag), "Tags");
            atc.SetFK("template", "Templates", "id");
            atc.SetFK("parent", "Tags", "id");
            return atc.GetQueryText();
        }
        private static string GetCommandDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Command), "Commands");
            atc.SetFK("database", "Databases", "path");
            atc.SetNotNull("database", true);
            atc.SetTextType("query");
            atc.SetTextType("restrictions");
            return atc.GetQueryText();
        }
        #endregion

        public static void AppendBackgroundQuery(Query q)
        {
            commandsText.Add(q.Result);
        }
        public async static void ExecuteBackground()
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
        public static void TryFix(SQLiteException ex, DBConnectionType con)
        {
            if (ex.Message.Contains("no such table:"))
            {
                string[] result = ex.Message.Split();
                AutoTableCreator atc = new AutoTableCreator();
                Query q = new Query("");
                q.typeOfConnection = con;
                switch (result[result.Length - 1].Trim())
                {
                    case "Processes":
                        q.AddCustomRequest(GetProcessDefinition(atc));
                        break;
                    case "Sectors":
                        q.AddCustomRequest(GetSectorDefinition(atc));
                        break;
                    case "Databases":
                        q.AddCustomRequest(GetDatabasesDefinition(atc));
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
                    case "Tasks":
                        q.AddCustomRequest(GetTaskDefinition(atc));
                        break;
                    case "Templates":
                        q.AddCustomRequest(GetTemplateDefinition(atc));
                        break;
                    case "GeneratedDocuments":
                        q.AddCustomRequest(GetGeneratedDocumentDefinition(atc));
                        break;
                    case "Tags":
                        q.AddCustomRequest(GetTagDefinition(atc));
                        break;
                    case "Commands":
                        q.AddCustomRequest(GetCommandDefinition(atc));
                        break;
                    default:
                        return;
                }
                q.ExecuteVoid();
                ProgramState.ShowInfoDialog("INCAS исправил ошибку. Программа будет перезапущена.");
                ServerProcessor.RestartProcessHandle(force: true);
            }
        }
    }
}

