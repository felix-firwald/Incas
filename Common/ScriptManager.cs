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
        public static string GetFullPathOfScript(string name)
        {
            return $"{ProgramState.Scripts}\\{name}";
        }
        public static void Execute(string fileName, UserControl currentForm)
        {
            // собственно среда выполнения Python-скрипта
            ScriptEngine engine = Python.CreateEngine();
            //engine.LoadAssembly(System.Reflection.Assembly.GetExecutingAssembly());
            
            try
            {
                engine.ExecuteFile(GetFullPathOfScript(fileName));
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"При выполнении пользовательского скрипта \"{fileName}\" возникла ошибка:\n" + ex.Message, "Ошибка выполнения скрипта");
            }
        }
    }
}
