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
        UNKNOWN,
        QUERY,
        RESPONSE
    }
    enum ProcessTarget
    {
        UNKNOWN,
        // FROM ADMIN-MODER ONLY
        TERMINATE = 101,
        RESTART = 102,
        EXPLICIT = 103,
        QUESTION = 104,
        GET_USER = 105,
        // ALL USERS
        OPEN_SEQUENCER = 201,
        COPY_FILE = 202,
        OPEN_WORD = 203,
        OPEN_EXCEL = 204,
        UPDATE_MAIN = 301,
         
    }
    enum ResponseCode
    {
        UNKNOWN,
        NO,
        OK
    }
    public struct ExplicitMessage
    {
        public string header;
        public string message;
    }
    struct Process
    {
        public string id;
        public string emitter;
        public string recipient;
        public ProcessType type;
        public ProcessTarget target;
        public string content;
    }
    static class ServerProcessor
    {
        public static string Port { get { return $"{ProgramState.ServerProcesses}\\Port {ProgramState.CurrentSession.slug}"; } }
        private static List<Process> WaitList = new();
        private static bool StopCreating = false;
        #region Reusable Base Functionality
        public static void StopPort()
        {
            StopCreating = true;
            if (Directory.Exists(ServerProcessor.Port))
            {
                string[] files = Directory.GetFiles(ServerProcessor.Port);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                Directory.Delete(ServerProcessor.Port);
            }
        }
        private static string GetKeyByRecipient(string recipient)
        {
            string result = string.Join("a", recipient.ToCharArray().Reverse()) + "b";
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
                string filename = $"{ProgramState.ServerProcesses}\\Port {process.recipient}\\{process.id}.procinc";
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
            catch (Exception)
            {
                return new Process();
            }
            
            File.Delete(filename);
            return result;
        }
        private static void RemoveOldest()
        {
            try
            {
                string[] dirs = Directory.GetDirectories(ProgramState.ServerProcesses);

                foreach (string dir in dirs)
                {
                    DirectoryInfo di = new DirectoryInfo(dir);
                    if (di.CreationTime < DateTime.Now.AddHours(-12) || !di.Name.StartsWith("Port"))
                    {
                        di.Delete();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }

        }
        private static void RuntimeCollector()
        {

        }
        #endregion

        public async static void Listen()
        {
            
            RemoveOldest();
            //string targetFileName = $"{ProgramState.ServerProcesses}\\{ProgramState.CurrentSession.slug}.procinc";
            await Task.Run(() =>
            {
                while (ProgramState.CurrentSession.active)
                {
                    try
                    {
                        if (!Directory.Exists(Port) && !StopCreating)
                        {
                            Directory.CreateDirectory(Port);
                        }
                        foreach (string f in Directory.GetFiles(Port))
                        {
                            Switcher(FromFile(f));
                        }
                        Thread.Sleep(200);
                        ProgramState.ShowUserSelector("Выберите пользователя, с которым хотите начать общение");
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                }
            });
        }
        public async static void Switcher(Process process)
        {
            await Task.Run(() =>
            {
                if (process.type == ProcessType.QUERY && process.recipient == ProgramState.CurrentSession.slug)
                {
                    switch (process.target)
                    {
                        case ProcessTarget.TERMINATE: // admin process
                            TerminateProcessHandle();
                            break;
                        case ProcessTarget.RESTART: // admin process
                            RestartProcessHandle();
                            break;
                        case ProcessTarget.EXPLICIT: // admin process
                            ShowExplicitMessageProcessHandle(process.content);
                            break;
                        case ProcessTarget.QUESTION:
                            ShowQuestionMessageProcessHandle(process.content);
                            break;
                        case ProcessTarget.UNKNOWN:
                        default: break;
                    }
                }
                else if (process.type == ProcessType.RESPONSE)
                {
                    
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
                    ProgramState.ClearDataForRestart();
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
        private static void ShowQuestionMessageProcessHandle(string content)
        {
            ExplicitMessage m = JsonConvert.DeserializeObject<ExplicitMessage>(content);
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgramState.ShowQuestionDialog(m.message, m.header);
            });
        }
        #endregion

        #region Sending
        private static string GenerateId()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return DateTime.Now.ToString("HHmmssffff") + new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static void AddToWaitList(Process process)
        {
            WaitList.Add(process);
        }
        private static void RemoveFromWaitList(Process process)
        {
            WaitList.Remove(process);
        }
        private static Process CreateResponseProcess(Process process)
        {
            (process.emitter, process.recipient) = (process.recipient, process.emitter);
            process.type = ProcessType.RESPONSE;
            return process;
        }
        private static Process CreateQueryProcess(string recipient)
        {
            Process process = new Process();
            
            process.id = GenerateId();
            process.emitter = ProgramState.CurrentSession.slug;
            process.recipient = recipient;
            process.type = ProcessType.QUERY;
            WaitList.Add(process);
            return process;
        }
        public static void SendTerminateProcess(string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.TERMINATE;
            ToFile(process);
        }
        public static void SendRestartProcess(string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.RESTART;
            ToFile(process);
        }
        public static void SendExplicitProcess(ExplicitMessage message, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.EXPLICIT;
            process.content = JsonConvert.SerializeObject(message);
            ToFile(process);
        }
        public static void SendQuestionProcess(ExplicitMessage message, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.QUESTION;
            process.content = JsonConvert.SerializeObject(message);
            AddToWaitList(process);
            ToFile(process);
        }
        #endregion
    }
}
