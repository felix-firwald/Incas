using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Incubator_2.Common
{
    public enum LogType
    {
        ERROR,
        ACTION,
        PROCESS
    }
    public static class Logger
    {
        private static string loggerFile { get { return $"{ProgramState.LogData}\\{ProgramState.CurrentSession.slug}.txt"; } }
        public async static void WriteLog(string text, LogType type = LogType.ACTION)
        {
            if (!string.IsNullOrEmpty(ProgramState.CurrentSession.slug))
            {
                if (ProgramState.CurrentUserParameters.write_log)
                {
                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            File.AppendAllText(loggerFile, $"\n[{DateTime.Now.ToString("T")}]\t{type}:\t{text}");
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(100);
                            WriteLog(text, type);
                        }
                    });
                }
            }
        }
    }
}
