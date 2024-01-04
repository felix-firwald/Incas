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


        public static bool CreateTables()
        {
            SQLiteConnection.CreateFile(ProgramState.DatabasePath);
            SQLiteConnection.CreateFile(ProgramState.ServiceDatabasePath);
            SQLiteConnection.CreateFile(ProgramState.CustomDatabasePath);
            //new Field(Field, )
            AutoTableCreator atc = new AutoTableCreator();
            Query q = new Query("");
            q.typeOfConnection = DBConnectionType.BASE;
            q.AddCustomRequest(GetParameterDefinition(atc))
             .AddCustomRequest(GetUserDefinition(atc))
             .AddCustomRequest(GetTaskDefinition(atc))
             .AddCustomRequest(GetSubtaskDefinition(atc))
             .AddCustomRequest(GetTemplateDefinition(atc))
             .AddCustomRequest(GetTagDefinition(atc))
             .AddCustomRequest(GetCustomTableDefinition(atc))
             .ExecuteVoid();
            q.typeOfConnection = DBConnectionType.SERVICE;
            q.AddCustomRequest(GetSessionDefinition(atc))
             .ExecuteVoid();
            return true;
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
            atc.SetAsUnique("password");
            atc.SetNotNull("fullname", false);
            atc.SetNotNull("surname", false);
            atc.SetNotNull("post", false);           
            return atc.GetQueryText();
        }
        private static string GetSessionDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Session), "Sessions");
            atc.SetNotNull("timeFinished", false);
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
            return atc.GetQueryText();
        }
        private static string GetTemplateDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Template), "Templates");
            atc.SetNotNull("suggestedPath", false);
            atc.SetNotNull("parent", false);
            atc.SetFK("parent", "Templates", "id");
            return atc.GetQueryText();
        }
        private static string GetTagDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Tag), "Tags");
            atc.SetNotNull("value", false);
            atc.SetNotNull("parent", false);
            atc.SetNotNull("parameters", false);
            atc.SetFK("template", "Templates", "id");
            atc.SetFK("parent", "Tags", "id");
            return atc.GetQueryText();
        }
        private static string GetCustomTableDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Subtask), "CustomTables");
            return atc.GetQueryText();
        }
    }
}

