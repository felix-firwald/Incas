using Common;
using DocumentFormat.OpenXml.Drawing;
using Incubator_2.Forms;
using Incubator_2.Forms.Templates;
using Incubator_2.Models;
using Incubator_2.Windows;
using Incubator_2.Windows.Templates;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

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
        OPEN_GENERATOR = 202,
        REQUEST_TEXT = 213,
        COPY_TEXT = 214,
        COPY_FILE = 215,
        OPEN_FILE = 216,
        OPEN_WEB = 217,
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
        public string back_id;
        public string emitter;
        public string recipient;
        public ProcessType type;
        public ProcessTarget target;
        public string content;
    }
    static class WaitControls
    {
        public static List<UC_TagFiller> TagFillers = new();
        public static Dictionary<string, Generator> Generators = new();
        public static Generator GetGenerator(string process)
        {
            Generator g = Generators[process];
            Generators.Remove(process);
            return g;
        }
    }

    static class ServerProcessor
    {
        public static string Port { get { return $"{ProgramState.ServerProcesses}\\{ProgramState.CurrentSession?.slug}.incport"; } }
        private static List<Process> WaitList = new();
        private static bool StopPulling = false;

        #region ControlsWait
        #endregion


        #region Base Functionality
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
                result = result.Substring(0, 31);
            }
            return result;
        }
        private async static void SendToPort(Process process)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
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
                return new Process();
            }
            return result;
        }
        private static void RemoveOldest()
        {
            try
            {
                List<Session> closed = new();
                using (Session s = new Session())
                {
                    closed = s.GetOpenedSessions(false);
                }
                foreach (Session s in closed)
                {    
                    File.Delete($"{ProgramState.ServerProcesses}\\{s.slug}.incport");
                }
            }
            catch (Exception e)
            {
                ProgramState.ShowErrorDialog(e.Message);
            }

        }
        #endregion

        #region Main
        public async static void Listen()
        {
            RemoveOldest();
            await System.Threading.Tasks.Task.Run(() =>
            {
                while (ProgramState.CurrentSession is not null || (bool)ProgramState.CurrentSession?.active || !StopPulling)
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
                                ProgramState.MainWindow.ProcessHandled = true;
                                Switcher(Parse(content));
                            }
                        }
                        ProgramState.CollectGarbage();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    Thread.Sleep(200);
                }
            });
        }
        public async static void Switcher(Process process)
        {
            await System.Threading.Tasks.Task.Run(() =>
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
                        case ProcessTarget.OPEN_SEQUENCER:
                            OpenSequencerProcessHandle(process.content);
                            break;
                        case ProcessTarget.OPEN_GENERATOR:
                            OpenGeneratorProcessHandle(process);
                            break;
                        case ProcessTarget.COPY_TEXT:
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                System.Windows.Clipboard.SetText(process.content);
                            });
                            break;
                        case ProcessTarget.COPY_FILE:
                            CopyFileProcessHandle(process.content);
                            break;
                        case ProcessTarget.OPEN_FILE:
                            OpenFileProcessHandle(process.content);
                            break;
                        case ProcessTarget.OPEN_WEB:
                            OpenWebProcessHandle(process.content);
                            break;
                        case ProcessTarget.REQUEST_TEXT:
                            ShowInputDialogHandle(process);
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
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                ProgramState.ShowInfoDialog($"Пользователь ответил: {process.content}", "Ответ на запрос");
                            });
                            break;
                        case ProcessTarget.REQUEST_TEXT:
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                string uid = process.content.Split("|||")[0];
                                string value = process.content.Split("|||")[1];
                                for (int i = 0; i <= WaitControls.TagFillers.Count; i++)
                                {
                                    if (WaitControls.TagFillers[i].GetId() == int.Parse(uid))
                                    {
                                        WaitControls.TagFillers[i].SetValue(value);
                                        WaitControls.TagFillers.Remove(WaitControls.TagFillers[i]);
                                        break;
                                    }
                                }
                            });
                            break;
                        case ProcessTarget.OPEN_GENERATOR:
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                WaitControls
                                .GetGenerator(process.back_id)
                                .SetData(JsonConvert.DeserializeObject<SGeneratedDocument>(process.content), "Требуется подтверждение");
                            });
                            break;
                    }
                }
                Thread.Sleep(500);
                ProgramState.MainWindow.ProcessHandled = false;
            });
            
        }
        #endregion

        #region Handling Queries
        private static void TerminateProcessHandle()
        {
            if (Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.Application.Current.MainWindow.Hide();
                    ProgramState.CloseSession();
                    System.Windows.Application.Current.MainWindow = new SessionBroken();
                    SessionBroken b = new SessionBroken();
                    b.ShowDialog();
                });
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        public static void RestartProcessHandle(bool force = false)
        {
            if (Permission.CurrentUserPermission != PermissionGroup.Admin || force)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.Application.Current.MainWindow.Hide();
                    ProgramState.CloseSession();
                    System.Windows.Application.Current.MainWindow = new SessionBroken();
                    SessionBroken b = new SessionBroken(BrokenType.Restart);
                    b.ShowDialog();
                    ProgramState.ClearDataForRestart();
                    System.Windows.Forms.Application.Restart();
                });
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        private static void CopyFileProcessHandle(string filename)
        {
            string fullname = $"{ProgramState.Exchanges}\\{filename}";
            FolderBrowserDialog fb = new();
            fb.Description = $"Папка для сохранения \"{filename}\"";
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(fullname, $"{fb.SelectedPath}\\{filename}", true);
                        File.Delete(fullname);
                        ProgramState.OpenFolder(fb.SelectedPath);
                    }
                    catch (Exception ex)
                    {
                        ProgramState.ShowErrorDialog($"При попытке выполнения процесса возникла ошибка:\n{ex}");
                    }
                }
            });

        }
        public static void OpenFileProcessHandle(string filename)
        {
            string fullname = $"{ProgramState.Exchanges}\\{filename}";
            try
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = fullname;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"Не удалось открыть присланный файл:\n{ex}", "Действие невозможно");
            }
        }
        private static void OpenWebProcessHandle(string url)
        {
            try
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = url;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"Не удалось открыть присланную ссылку:\n{ex}", "Действие невозможно");
            }
        }
        private static void OpenSequencerProcessHandle(string content)
        {
            try
            {
                List<SGeneratedDocument> documents = JsonConvert.DeserializeObject<List<SGeneratedDocument>>(content);
                using (Template t = new())
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        UseTemplate ut = new(t.GetTemplateById(documents[0].template), documents);
                        ut.Show();
                    });
                }
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"Не удалось открыть присланную запись:\n{ex}", "Действие невозможно");
            }
        }
        private static void OpenGeneratorProcessHandle(Process p)
        {
            try
            {
                SGeneratedDocument part = JsonConvert.DeserializeObject<SGeneratedDocument>(p.content);
                using (Template t = new())
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        UseTemplateText ut = new(t.GetTemplateById(part.template), part);
                        ut.Show();
                        ut.OnFinishedEditing += new(() =>
                        {
                            SendOpenGeneratorResultResponse(p, ut.GetData());
                            return;
                        });

                    });
                }
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"Не удалось открыть присланную часть документа:\n{ex}", "Действие невозможно");
            }
        }


        private static void ShowExplicitMessageProcessHandle(string content, Process process)
        {
            ExplicitMessage m = JsonConvert.DeserializeObject<ExplicitMessage>(content);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
        private static void ShowInputDialogHandle(Process oldProc)
        {
            Process process = new();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                string result = ProgramState.ShowInputBox("Запрос значения", oldProc.content.Split("|||")[1]);
                process.content = $"{oldProc.content.Split("|||")[0]}|||{result}";
            });
            process.id = GenerateId();
            process.emitter = ProgramState.CurrentSession.slug;
            process.recipient = oldProc.emitter;
            process.target = ProcessTarget.REQUEST_TEXT;
            process.type = ProcessType.RESPONSE;
            SendToPort(process);
        }
        #endregion

        #region Sending
        private static string GenerateId()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return DateTime.Now.ToString("HHmmssffff") + new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static Process CreateResponseProcess(Process process)
        {
            (process.emitter, process.recipient) = (process.recipient, process.emitter);
            process.back_id = process.id;
            process.id = GenerateId();
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
            SendToPort(process);
        }
        public static void SendRestartProcess(string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.RESTART;
            SendToPort(process);
        }
        public static void SendExplicitProcess(ExplicitMessage message, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.EXPLICIT;
            process.content = JsonConvert.SerializeObject(message);
            SendToPort(process);
        }
        public static void SendCopyTextProcess(string text, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.COPY_TEXT;
            process.content = text;
            SendToPort(process);
        }
        public static void SendRequestTextProcess(UC_TagFiller tagfiller, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.REQUEST_TEXT;
            process.content = $"{tagfiller.GetId()}|||{tagfiller.tag.name}";
            WaitControls.TagFillers.Add(tagfiller);
            SendToPort(process);
        }
        private static void SendFileProcess(string filename, string fullname, string recipient, ProcessTarget target)
        {
            try
            {
                Process process = CreateQueryProcess(recipient);
                process.target = target;
                process.content = filename;
                if (!fullname.StartsWith(ProgramState.Exchanges))
                {
                    File.Copy(fullname, $"{ProgramState.Exchanges}\\{filename}", true);
                }
                SendToPort(process);
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog(ex.Message);
            }
        }
        public static void SendCopyFileProcess(string filename, string fullname, string recipient)
        {
            SendFileProcess(filename, fullname, recipient, ProcessTarget.COPY_FILE);
        }
        public static void SendOpenFileProcess(string filename, string fullname, string recipient)
        {
            SendFileProcess(filename, fullname, recipient, ProcessTarget.OPEN_FILE);
        }
        public static void SendOpenWebProcess(string url, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.OPEN_WEB;
            process.content = url;
            SendToPort(process);
        }
        public static void SendOpenSequencerProcess(List<SGeneratedDocument> documents, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.OPEN_SEQUENCER;
            process.content = JsonConvert.SerializeObject(documents);
            SendToPort(process);
        }
        public static void SendOpenGeneratorProcess(SGeneratedDocument part, Generator tagfiller, string recipient)
        {
            Process process = CreateQueryProcess(recipient);
            process.target = ProcessTarget.OPEN_GENERATOR;
            process.content = JsonConvert.SerializeObject(part);
            WaitControls.Generators.Add(process.id, tagfiller);
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
        private static void SendOpenGeneratorResultResponse(Process inputProcess, SGeneratedDocument part)
        {
            Process outputProcess = CreateResponseProcess(inputProcess);
            outputProcess.content = JsonConvert.SerializeObject(part);
            SendToPort(outputProcess);
        }
        #endregion
    }
}
