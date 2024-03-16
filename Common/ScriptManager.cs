using Common;
using Incubator_2.Windows.CustomDatabase;
using IronPython.Hosting;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Incas
{
    public class Service
    {
        public Service() { }
        public static User CurrentUser = ProgramState.CurrentUser;
        public static string CurrentSessionId = ProgramState.CurrentSession.slug;
        public static void ShowInfoDialog(string message)
        {
            ProgramState.ShowInfoDialog(message);
        }
        public static string ShowInputBox(string title, string description)
        {
            return ProgramState.ShowInputBox(title, description);
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
            Dictionary<string, string> result = new();
            foreach (DataColumn dataColumn in ds.SelectedValues.Table.Columns)
            {
                result.Add(dataColumn.ColumnName, ds.SelectedValues[dataColumn.ColumnName].ToString());
            }
            return result;
        }
        public static List<string> GetCategoriesOfTemplates()
        {
            using (Template t = new())
            {
                return t.GetCategories();
            }
        }
        public static List<STemplate> GetWordTemplates(string category)
        {
            using (Template t = new())
            {
                return t.GetWordTemplates(category);
            }
        }
        public static List<STemplate> GetWordTemplates()
        {
            using (Template t = new())
            {
                return t.GetAllWordTemplates();
            }
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
                
                ProgramState.ShowWaitCursor(false);
            }
            catch (Exception ex)
            {
                ProgramState.ShowWaitCursor(false);
                ProgramState.ShowErrorDialog($"При выполнении пользовательского скрипта возникла ошибка:\n" + ex.Message, "Ошибка выполнения скрипта");
            }
        }
    }
}
