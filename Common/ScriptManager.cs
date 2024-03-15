using Common;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
            // собственно среда выполнения Python-скрипта
            //ScriptEngine engine = Python.CreateEngine();
            //engine.Runtime.LoadAssembly(typeof(string).Assembly);
            //scope.SetVariable("input_var", "13.03.2024");
            //engine.LoadAssembly(System.Reflection.Assembly.GetExecutingAssembly());
            
            try
            {
                engine.Execute(script, scope);
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"При выполнении пользовательского скрипта возникла ошибка:\n" + ex.Message, "Ошибка выполнения скрипта");
            }
        }
    }
}
