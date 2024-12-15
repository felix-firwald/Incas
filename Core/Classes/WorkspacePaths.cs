using System;
using System.IO;

namespace Incas.Core.Classes
{
    internal static class WorkspacePaths
    {
        internal static string UserPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Incas";
        internal static string UserPathLicense { get { return UserPath + "License\\"; } }
        internal static string UserPathLogs { get { return UserPath + "Logs\\"; } }
        internal static void SetCommonPath(string path, bool checkout = true)
        {
            ProgramState.CurrentWorkspace = new(path);
            if (checkout)
            {
                DatabaseManager.ActualizeTables();
            }
            ProgramState.CollectGarbage();
        }
    }
}