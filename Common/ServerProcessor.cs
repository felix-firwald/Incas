using Common;
using DocumentFormat.OpenXml.Presentation;
using Incubator_2.Models;
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
using Windows.ApplicationModel.Background;

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
    public enum AdminMessageType
    {
        NOTIFY,
        WARNING,
        QUESTION
    }
    public struct ExplicitMessage
    {
        public string header;
        public string message;
        public AdminMessageType message_type;
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
        public static string Port { get { return $"{ProgramState.ServerProcesses}\\{ProgramState.CurrentSession.slug}.incport"; } }
        private static List<Process> WaitList = new();
        private static bool StopPulling = false;
        #region Reusable Base Functionality
        public static void StopPort()
        {
            StopPulling = true;
            try
            {
                File.Delete(Port);
            }
            catch { }
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
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    ProgramState.ShowInfoDialog(result.Length.ToString());
                //});
                result = result.Substring(0, 31);
            }
            return result;
        }
        private async static void SendToPort(Process process)
        {
            await Task.Run(() =>
            {
                Logger.WriteLog($"Encrypting process {process.id}");
                string content = JsonConvert.SerializeObject(process);
                using (Models.Process p = new())
                {
                    p.identifier = process.id;
                    p.content = Cryptographer.EncryptString(GetKeyByRecipient(process.recipient), content);
                    p.Send(process.recipient);
                }
            });
        }
        private static Process Parse(string input)
        {
            Process result = new Process();
            try
            {
                string output = Cryptographer.DecryptString(GetKeyByRecipient(ProgramState.CurrentSession.slug), input);
                result = JsonConvert.DeserializeObject<Process>(output);
            }
            catch (Exception e)
            {
                ProgramState.ShowErrorDialog(e.Message);
                Logger.WriteLog(e.Message, LogType.ERROR);
                return new Process();
            }
            return result;
        }
        private static void RemoveOldest()
        {
            try
            {
                string[] files = Directory.GetFiles(ProgramState.ServerProcesses);

                foreach (string dir in files)
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
        #endregion

        public async static void Listen()
        {
            RemoveOldest();
            await Task.Run(() =>
            {
                while (ProgramState.CurrentSession is not null || ProgramState.CurrentSession.active || !StopPulling)
                {
                    try
                    {
                        if (!File.Exists(Port))
                        {
                            DatabaseManager.InitializePort();
                        }
                        using (Models.Process process = new())
                        {
                            foreach (string content in process.GetNewProcesses())
                            {
                                Switcher(Parse(content));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog(e.Message, LogType.ERROR);
                        continue;
                    }
                    Thread.Sleep(200);
                }
            });
        }
        public async static void Switcher(Process process)
        {
            Logger.WriteLog($"Process was received: {process.id} {process.target} {process.content}");
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
                            ShowExplicitMessageProcessHandle(process.content, process);
                            break;
                        case ProcessTarget.UNKNOWN:
                        default: break;
                    }
                }
                else if (process.type == ProcessType.RESPONSE)
                {
                    switch (process.target)
                    {
                        case ProcessTarget.EXPLICIT:
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ProgramState.ShowInfoDialog($"Пользователь ответил: {process.content}", "Ответ на запрос");
                            });
                            break;
                    }
                }
            });
            
        }
        private static Process GetProcessFromWaitList(string id)
        {
            foreach (Process process in WaitList)
            {
                if (process.id == id)
                {
                    return process;
                }
            }
            return new Process();
        }
        private static void RemoveProcessFromWaitList(string id)
        {
            foreach (Process process in WaitList)
            {
                if (process.id == id)
                {
                    WaitList.Remove(process);
                }
            }
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
        private static void ShowExplicitMessageProcessHandle(string content, Process process)
        {
            ExplicitMessage m = JsonConvert.DeserializeObject<ExplicitMessage>(content);
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (m.message_type)
                {
                    case AdminMessageType.NOTIFY:
                    default:
                        ProgramState.ShowInfoDialog(m.message, m.header);
                        break;
                    case AdminMessageType.WARNING:
                        ProgramState.ShowExclamationDialog(m.message, m.header);
                        break;
                    case AdminMessageType.QUESTION:
                        DialogStatus ds = ProgramState.ShowQuestionDialog(m.message, m.header);
                        SendQuestionResultResponse(process, ds);
                        break;
                }
                
            });
        }
        #endregion

        #region Sending
        private static void WriteLogSending(Process process)
        {
            Logger.WriteLog($"Process send: id {process.id}, type {process.type}, target {process.target}");
        }
        private static string GenerateId()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return DateTime.Now.ToString("HHmmssffff") + new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
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
            return process;
        }
        public static void SendTerminateProcess(string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.TERMINATE;
            WriteLogSending(process);
            SendToPort(process);
        }
        public static void SendRestartProcess(string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.RESTART;
            WriteLogSending(process);
            SendToPort(process);
        }
        public static void SendExplicitProcess(ExplicitMessage message, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.EXPLICIT;
            process.content = JsonConvert.SerializeObject(message);
            WriteLogSending(process);
            SendToPort(process);
        }

        #endregion
        #region Responses
        private static void SendQuestionResultResponse(Process inputProcess, DialogStatus ds)
        {
            Process outputProcess = CreateResponseProcess(inputProcess);
            outputProcess.content = (ds == DialogStatus.Yes) ? "Да" : "Нет";
            SendToPort(outputProcess);
        }
        #endregion
    }
}
