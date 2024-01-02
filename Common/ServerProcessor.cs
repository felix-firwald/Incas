using Common;
using Incubator_2.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
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
        EXPLICIT,
        FILE
    }
    public struct ExplicitMessage
    {
        public string header;
        public string message;
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
        #region Reusable Base Functionality
        private static string GetKeyByRecipient(string recipient)
        {
            string result = string.Join("e", recipient.ToCharArray().Reverse()) + "b";
            if (result.Length < 32)
            {
                int multiplier = 32 - result.Length;
                result += Enumerable.Repeat("a", multiplier);
            }
            else if (result.Length > 32)
            {
                result = result.Substring(0, 31);
            }
            return result;
        }
        private async static void ToFile(Process process)
        {
            await Task.Run(() =>
            {
                string content = JsonConvert.SerializeObject(process);
                string filename = $"{ProgramState.ServerProcesses}\\{process.recipient}.procinc";
                File.WriteAllTextAsync(filename, Cryptographer.EncryptString(GetKeyByRecipient(process.recipient), content));
            });
        }
        private static Process FromFile(string filename)
        {
            Process result = new Process();
            try
            {
                string output = Cryptographer.DecryptString(GetKeyByRecipient(ProgramState.CurrentSession.slug), File.ReadAllText(filename));
                result = JsonConvert.DeserializeObject<Process>(output);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgramState.ShowErrorDialog($"Файл процесса был поврежден или подделан.\nПроцесс не будет выполнен. Сведения:\n{ex}", "Ошибка выполнения процесса");
                });
                File.Delete(filename);
                return new Process();
            }
            
            File.Delete(filename);
            return result;
        }
        #endregion

        public async static void Listen()
        {
            string targetFileName = $"{ProgramState.ServerProcesses}\\{ProgramState.CurrentSession.slug}.procinc";
            await Task.Run(() =>
            {
                while (ProgramState.CurrentSession.active)
                {
                    Thread.Sleep(750);
                    if (File.Exists(targetFileName))
                    {
                        Switcher(FromFile(targetFileName));
                    }
                }
            });
        }
        public async static void Switcher(Process process)
        {
            await Task.Run(() =>
            {
                if (process.type == ProcessType.QUERY)
                {
                    switch (process.target)
                    {
                        case ProcessTarget.TERMINATE:
                            TerminateProcessHandle();
                            break;
                        case ProcessTarget.RESTART:
                            RestartProcessHandle();
                            break;
                        case ProcessTarget.EXPLICIT:
                            ShowExplicitMessageProcessHandle(process.content);
                            break;
                    }
                }
            });
            
        }
        #region Handling Queries
        private static void TerminateProcessHandle()
        {
            if (Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.MainWindow.Hide();
                    ProgramState.CloseSession();
                    Application.Current.MainWindow = new SessionBroken();
                    SessionBroken b = new SessionBroken();
                    b.ShowDialog();
                });
            }
        }
        private static void RestartProcessHandle()
        {
            if (Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.MainWindow.Hide();
                    ProgramState.CloseSession();
                    Application.Current.MainWindow = new SessionBroken();
                    SessionBroken b = new SessionBroken(BrokenType.Restart);
                    b.ShowDialog();
                    System.Windows.Forms.Application.Restart();
                });
            }
        }
        private static void ShowExplicitMessageProcessHandle(string content)
        {
            ExplicitMessage m = JsonConvert.DeserializeObject<ExplicitMessage>(content);
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.ShowExlamationDialog(m.message, m.header);
            });
        }
        #endregion

        #region Sending
        private static Process GetProcessStruct(string recipient)
        {
            Process process = new Process();
            process.emitter = ProgramState.CurrentSession.slug;
            process.recipient = recipient;
            process.type = ProcessType.QUERY;
            return process;
        }
        public static void SendTerminateProcess(string recipient)
        {
            Process process = GetProcessStruct(recipient);
            process.target = ProcessTarget.TERMINATE;
            ToFile(process);
        }
        public static void SendRestartProcess(string recipient)
        {
            Process process = GetProcessStruct(recipient);
            process.target = ProcessTarget.RESTART;
            ToFile(process);
        }
        public static void SendExplicitProcess(ExplicitMessage message, string recipient)
        {
            Process process = GetProcessStruct(recipient);
            process.target = ProcessTarget.EXPLICIT;
            process.content = JsonConvert.SerializeObject(message);
            ToFile(process);
        }
        #endregion
    }
}
