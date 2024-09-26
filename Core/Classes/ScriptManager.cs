using Incas.Core.Classes;
using Incas.CustomDatabases.Views.Windows;
using Incas.Objects.Views.Windows;
using Incas.Templates.Models;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Core.Classes
{
    public class Service
    {
        public Service() { }
        public static string CurrentSessionId = ProgramState.CurrentSession.slug;
        public static string GetUserUsername()
        {
            return ProgramState.CurrentUser.username;
        }
        public static string GetUserFullname()
        {
            return ProgramState.CurrentUser.fullname;
        }
        public static void ShowInfoDialog(string message, string title)
        {
            DialogsManager.ShowInfoDialog(message, title);
        }
        public static string ShowInputBox(string title, string description)
        {
            return DialogsManager.ShowInputBox(title, description);
        }
        public static string ShowDatabaseSelection(string db, string table, string field)
        {
            DatabaseSelection ds = new(db, table, field);
            ds.ShowDialog();
            return ds.SelectedValue;
        }
        public static Dictionary<string, string> ShowDatabaseSelection(string db, string table)
        {
            DatabaseSelection ds = new(db, table, "");
            ds.ShowDialog();
            Dictionary<string, string> result = [];
            foreach (DataColumn dataColumn in ds.SelectedValues.Table.Columns)
            {
                result.Add(dataColumn.ColumnName, ds.SelectedValues[dataColumn.ColumnName].ToString());
            }
            return result;
        }
        public static List<string> GetCategoriesOfTemplates()
        {
            using Template t = new();
            return t.GetCategories(["Excel", "Word"]);
        }
        public static List<STemplate> GetWordTemplates(string category)
        {
            using Template t = new();
            return t.GetWordTemplates(category);
        }
        public static List<STemplate> GetWordTemplates()
        {
            using Template t = new();
            return t.GetAllWordTemplates();
        }
    }
}
namespace Incubator_2.Common
{

    public static class ScriptManager
    {
        private static ScriptEngine engine;
        static ScriptManager()
        {
            engine = Python.CreateEngine();

        }
        public static ScriptEngine GetEngine()
        {
            return engine;
        }
        public static string GetFullPathOfScript(string name)
        {
            return $"{ProgramState.Scripts}\\{name}";
        }
        public static void Execute(string script, ScriptScope scope)
        {
            try
            {
                //engine.Runtime.LoadAssembly(typeof(ProgramState).Assembly);
                string minimum = "";
                if (script.Contains("from Incas import Service"))
                {
                    scope.ImportModule("clr");
                    minimum = "clr.AddReference('Incas')\n";
                }
                engine.Execute(minimum + script, scope);
                DialogsManager.ShowWaitCursor(false);
            }
            catch (Exception ex)
            {
                DialogsManager.ShowWaitCursor(false);
                DialogsManager.ShowErrorDialog($"При выполнении пользовательского скрипта возникла ошибка:\n" + ex.Message, "Ошибка выполнения скрипта");
            }
        }
    }
}
