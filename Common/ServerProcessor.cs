using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Incubator_2.Common
{
    enum ProcessType
    {
        QUERY,
        RESPONSE
    }
    enum ProcessTarget
    {
        TERMINATE,
        RESTART,
        MESSAGE,
        FILE
    }
    struct Process
    {
        public string emitter;
        public string recipient;
        public ProcessType type;
        public ProcessTarget target;
        public string content;
    }
    static class ServerProcessor
    {
        private static void ToFile(Process process)
        {
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(process);
            string filename = $"{ProgramState.ServerProcesses}\\{process.recipient}.procinc";
            File.WriteAllTextAsync(filename, content);
        }
        private static Process FromFile(string filename)
        {
            Process result = Newtonsoft.Json.JsonConvert.DeserializeObject<Process>(File.ReadAllText(filename));
            File.Delete(filename);
            return result;
        }
        public static void SendTerminateProcess(string recipient)
        {
            if (Permission.CurrentUserPermission == PermissionGroup.Admin)
            {
                Process process = new Process();
                process.emitter = ProgramState.CurrentSession.slug;
                process.recipient = recipient;
                process.type = ProcessType.QUERY;
                process.target = ProcessTarget.TERMINATE;
                ToFile(process);
            }
        }
        
        public async static void Listen()
        {
            string targetFileName = $"{ProgramState.ServerProcesses}\\{ProgramState.CurrentSession.slug}.procinc";
            await Task.Run(() =>
            {
                while (ProgramState.CurrentSession.active)
                {
                    Thread.Sleep(1000);
                    if (File.Exists(targetFileName))
                    {
                        Switcher(FromFile(targetFileName));
                    }
                    else
                    {

                        //Application.Current.Dispatcher.Invoke(() => 
                        //{
                        //    ProgramState.ShowExlamationDialog("Файл не найден!");
                        //});
                    }
                }
            });
        }
        public async static void Switcher(Process process)
        {
            if (process.type == ProcessType.QUERY)
            {
                switch (process.target)
                {
                    case ProcessTarget.TERMINATE:
                        if (Permission.CurrentUserPermission != PermissionGroup.Admin)
                        {
                            ProgramState.CloseSession();
                            Application.Current.Shutdown();
                        }
                        break;
                }
            }
            
        }
    }
}
