﻿using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;

namespace Incas.Core.Classes
{
    public class Service
    {
        public Service() { }
        public static void ShowInfoDialog(string message, string title)
        {
            DialogsManager.ShowInfoDialog(message, title);
        }
    }

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
