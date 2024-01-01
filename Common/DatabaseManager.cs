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
        readonly public static string path = ProgramState.DatabasePath;
        public static SQLiteConnection connection;
        public static void OpenConnection()
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

        public static bool CreateTables(string path)
        {
            SQLiteConnection.CreateFile(path);
            //new Field(Field, )
            AutoTableCreator atc = new AutoTableCreator();
            Query q = new Query("");
            q.AddCustomRequest(GetParameterDefinition(atc))
             .AddCustomRequest(GetUserDefinition(atc))
             .AddCustomRequest(GetComputerDefinition(atc))
             .AddCustomRequest(GetSessionDefinition(atc))
             .AddCustomRequest(GetTaskDefinition(atc))
             .AddCustomRequest(GetSubtaskDefinition(atc))
             .AddCustomRequest(GetTemplateDefinition(atc))
             .AddCustomRequest(GetTagDefinition(atc))
             .AddCustomRequest(GetCustomTableDefinition(atc))
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
        private static string GetComputerDefinition(AutoTableCreator atc)
        {
            atc.Initialize(typeof(Computer), "Computers");
            atc.SetAsUnique("authId");
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

