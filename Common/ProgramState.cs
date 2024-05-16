using IncasEngine.TemplateManager;
using Incubator_2.Common;
using Incubator_2.Models;
using Incubator_2.ViewModels;
using Incubator_2.Windows;
using Incubator_2.Windows.CustomDatabase;
using Incubator_2.Windows.Selectors;
using Microsoft.Win32;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Common
{
    public class LockedException : Exception
    {
        public LockedException() { }
    }
    public class UserAlreadyOnlineException : Exception
    {
        public UserAlreadyOnlineException() { }
    }
    public class SessionBrokenException : Exception
    {
        public SessionBrokenException() { }
    }

    internal struct FirstWorkspaceData
    {
        public string workspacePath;
        public string workspaceName;
        public string userSurname;
        public string userFullname;
        public string userPassword;
    }

    internal static class ProgramState
    {
        public static string CommonPath { get; private set; }
        public static string UserPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Incas";
        public static string DatabasePath { get; private set; }
        public static string CustomDatabasePath { get { return Root + @"\Databases"; } }
        public static string ServiceDatabasePath { get { return Root + @"\service.dbinc"; } }
        public static string Root { get { return CommonPath + @"\Root"; } }
        public static string ServerProcesses { get { return Root + @"\ServerProcesses"; } } // ...\Root\ServerProccesses
        public static string Exchanges { get { return Root + @"\Exchanges"; } } // ...\Root\Exchanges
        public static string Messages { get { return Root + @"\Messages"; } } // папка еще не создана

        public static string Scripts { get { return Root + @"\Scripts"; } }
        public static string LogData { get { return Root + @"\LogData"; } } // ...\Root\LogData

        #region Templates

        private static string Templates { get { return Root + @"\Templates"; } }    // ...\Root\Templates
        public static string TemplatesSourcesWordPath { get { return Templates + @"\Sources\Word"; } }    // ...\Root\Templates\Sources\Word
        public static string TemplatesSourcesExcelPath { get { return Templates + @"\Sources\Excel"; } }    // ...\Root\Templates\Sources\Excel
        public static string TemplatesRuntime { get { return Templates + @"\Runtime"; } }    // ...\Root\Templates\Runtime
        #endregion

        #region User
        public static User CurrentUser { get; set; }
        public static UserParameters CurrentUserParameters { get; set; }
        public static Session CurrentSession { get; private set; }
        #endregion
        public static Sector CurrentSector { get; private set; }
        public static MV_MainWindow MainWindow { get; set; }

        private static DateTime LastGarbageCollect = DateTime.Now;
        #region Path and init
        public static void SetCommonPath(string path, bool checkout = true)
        {
            CommonPath = path;
            DatabasePath = path + @"\data.dbinc";
            Directory.CreateDirectory(Templates);
            Directory.CreateDirectory(TemplatesSourcesWordPath);
            Directory.CreateDirectory(TemplatesSourcesExcelPath);
            Directory.CreateDirectory(ServerProcesses);
            Directory.CreateDirectory(Scripts);
            Directory.CreateDirectory(Exchanges);
            Directory.CreateDirectory(CustomDatabasePath);
            Directory.CreateDirectory(Messages);
            Directory.CreateDirectory(LogData);
            Directory.CreateDirectory(TemplatesRuntime);
            if (checkout)
            {
                DatabaseManager.ActualizeTables();
            }
            CollectGarbage();
            ScriptManager.Execute("from Incas import Service", ScriptManager.GetEngine().CreateScope());
            //TelegramProcessor.StartBot("6911917508:AAHJeEhfNKzzOJjp0IlGtZ51lqNrE2LBnK4");
        }
        public static string GetFullPathOfCustomDb(string path)
        {
            return $"{CustomDatabasePath}\\{path}.db";
        }
        public static void ClearDataForRestart()
        {
            CurrentUser = null;
            CurrentSession = null;
            CommonPath = null;
            Permission.CurrentUserPermission = PermissionGroup.Operator;
        }
        public static bool CheckSensitive()
        {
            if (CurrentUser == null || CurrentSession == null)
            {
                return false;
            }
            return true;
        }
        public static bool IsCommonPathExists()
        {
            return Directory.Exists(CommonPath);
        }
        private static string getDBFilePath()
        {
            return CommonPath + @"\data.dbinc";
        }
        public static void GetDBFile()
        {
            if (!File.Exists(getDBFilePath()))
            {
                Dialog q = new Dialog("По указанному пути рабочее пространство не обнаружено.\n" +
                "Проверьте правильность введенного пути или создайте новое рабочее пространство.", "Рабочее пространство не обнаружено");
                q.ShowDialog();
            }
        }
        #endregion

        #region ComputerId
        public static string GenerateSlug(int len, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, len)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static bool IsRegistryContainsData()
        {
            return Registry.CurrentUser.GetSubKeyNames().Contains("Incubator");
        }

        #endregion

        #region UserData
        public static bool LoadUserData()
        {
            if (IsRegistryContainsData())
            {
                SetCommonPath(RegistryData.GetSelectedWorkspacePath());
                return true;
            }
            return false;
        }
        #endregion

        #region WorkspaceFiles
        public static string GetFullnameOfWordFile(string name)
        {
            return TemplatesSourcesWordPath + "\\" + name;
        }
        public static string GetFullnameOfExcelFile(string name)
        {
            return TemplatesSourcesExcelPath + "\\" + name;
        }
        #endregion
        private static bool CreateTablesInDatabase()
        {
            return DatabaseManager.InitializeService();
        }
        public static Parameter GetParameter(ParameterType type, string name, string defaultValue = "0", bool createIfNot = true)
        {
            Parameter par = new Parameter();
            par.GetParameter(type, name, defaultValue, createIfNot);
            return par;
        }

        #region Incubator
        public static void InitWorkspace(FirstWorkspaceData data)
        {
            int recursion = 0;
            void Create()
            {
                try
                {
                    recursion++;

                    using (Parameter par = new())
                    {
                        par.type = ParameterType.INCUBATOR;
                        par.name = "ws_name";
                        par.value = data.workspaceName;
                        par.CreateParameter();
                        par.name = "ws_opened";
                        par.WriteBoolValue(true);
                        par.CreateParameter();
                        par.name = "ws_locked";
                        par.WriteBoolValue(false);
                        par.CreateParameter();

                    }
                    ShowInfoDialog("Рабочее пространство успешно создано.");
                    using (Sector sector = new())
                    {
                        sector.slug = "data";
                        sector.name = "Базовый сектор";
                        sector.AddSector(false);
                    }

                    using User user = new();
                    user.username = "admin";
                    user.post = "Администратор рабочего пространства";
                    user.surname = data.userSurname;
                    user.secondName = data.userFullname;
                    user.fullname = $"{user.surname} {user.secondName}";
                    user.sector = "data";
                    UserParameters up = new();
                    up.permission_group = PermissionGroup.Admin;
                    up.tasks_visibility = true;
                    up.communication_visibility = true;
                    up.database_visibility = true;
                    up.password = data.userPassword;
                    user.GenerateSign();
                    user.SaveParametersContext(up);
                    user.AddUser();
                }
                catch (IOException)
                {
                    Thread.Sleep(50);
                    if (recursion < 20)
                    {
                        Create();
                    }
                    else
                    {
                        ShowErrorDialog("Нечто блокирует файл базы данных.");
                    }
                }
            }
            ShowWaitCursor();
            Permission.CurrentUserPermission = PermissionGroup.Admin;
            SetCommonPath(data.workspacePath, false);
            if (CreateTablesInDatabase())
            {
                ShowWaitCursor(false);
                RegistryData.SetWorkspacePath(data.workspaceName, data.workspacePath);
                Create();
            }
        }
        public static string GetWorkspaceName()
        {
            using Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_name", "Рабочая область");
            return par.value;
        }
        public static void SetWorkspaceName(string name)
        {
            using Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_name", "Рабочая область");
            par.value = name;
            par.UpdateValue();
        }
        public static void SetWorkspaceOpened(bool opened)
        {
            using Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_opened", "1");
            par.WriteBoolValue(opened);
            par.UpdateValue();
        }
        public static bool IsWorkspaceOpened()
        {
            if (Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                using Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_opened", "1");
                bool result = par.GetValueAsBool();
                if (!result)
                {
                    ShowErrorDialog("Действия по добавлению, изменению " +
                        "или удалению информации из базы данных недоступны, " +
                        "пока рабочее пространство находится в статусе \"Закрыто\".\n" +
                        "Рабочее пространство по-прежнему можно использовать, однако " +
                        "только для чтения.", "Рабочее пространство закрыто");
                }
                return result;
            }
            else
            {
                return true;
            }
        }
        public static void SetWorkspaceLocked(bool locked)
        {
            using Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_locked");
            par.WriteBoolValue(locked);
            par.UpdateValue();
        }
        public static bool IsWorkspaceLocked()
        {
            using Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_locked");
            return par.GetValueAsBool();
        }

        #endregion
        public static void CheckLocked()
        {
            if (IsWorkspaceLocked() && Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                throw new LockedException();
            }
        }

        #region Session

        public static void BrokeSession()
        {
            throw new SessionBrokenException();
        }
        public static void OpenSession()
        {
            using Session ms = new Session();
            ms.AddSession();
            CurrentSession = ms;
            ms.ClearOldestSessions();
            //Directory.CreateDirectory(ServerProcessor.Port);
        }
        public static List<Session> GetActiveSessions()
        {
            using Session ms = new Session();
            return ms.GetOpenedSessions();
        }
        public static void CloseSession()
        {
            if (CurrentSession != null && CurrentSession.active)
            {
                CurrentSession.CloseSession();
            }
            ServerProcessor.StopPort();
        }
        #endregion
        public static void PlaySound(string path)
        {
            try
            {
                using FileStream stream = File.Open($"Static\\{path}.wav", FileMode.Open);
                SoundPlayer myNewSound = new SoundPlayer(stream);
                myNewSound.Load();
                myNewSound.Play();
                myNewSound.Dispose();
            }
            catch { }
        }
        public static void OpenWebPage(string url)
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
                ProgramState.ShowErrorDialog($"Не удалось открыть присланную ссылку:\n{ex.Message}", "Действие невозможно");
            }
        }
        #region Modal Dialogs
        public static void ShowErrorDialog(string message, string title = "Возникла неизвестная ошибка")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (message == null)
                {
                    return;
                }
                PlaySound("UI-Exclamation");
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.Error);
                d.ShowDialog();
            });
        }
        public static void ShowDatabaseErrorDialog(string message, string title = "Ошибка при выполнении запроса")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlaySound("UI-Exclamation");
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.DatabaseError);
                d.ShowDialog();
            });
        }
        public static void ShowAccessErrorDialog(string message, string title = "Нет доступа")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlaySound("UI-Exclamation");
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.AccessDenied);
                d.ShowDialog();
            });
        }
        public static void ShowExclamationDialog(string message, string title = "Обратите внимание")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlaySound("UI-Attention");
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.Exclamation);
                d.ShowDialog();
            });
        }
        public static void ShowInfoDialog(string message, string title = "Оповещение")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlaySound("UI-Attention");
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.Info);
                d.ShowDialog();
            });
        }
        // for dev only
        public static void ShowInfoDialog(object message, string title = "Оповещение")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlaySound("UI-Attention");
                Dialog d = new Dialog(message.ToString(), title, Dialog.DialogIcon.Info);
                d.ShowDialog();
            });
        }
        public static DialogStatus ShowQuestionDialog(string message, string title, string yesText = "Да", string noText = "Нет")
        {
            DialogQuestion d = new DialogQuestion(message, title, yesText, noText);
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlaySound("UI-Attention");
                d.ShowDialog();
            });
            return d.status;
        }
        public static string ShowInputBox(string title, string description = "Введите значение")
        {
            DialogInput dialog = new(title, description);
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialog.ShowDialog();
            });
            return dialog.Input;
        }
        public static void ShowWindow(UserControl control, string title)
        {
            ContainerWindow cw = new(control, title);
            cw.Show();
        }
        public static void ShowWindowDialog(UserControl control, string title)
        {
            ContainerWindow cw = new(control, title);
            cw.ShowDialog();
        }
        public static BindingSelector ShowBindingSelector()
        {
            BindingSelector bd = new();
            Application.Current.Dispatcher.Invoke(() =>
            {
                bd.ShowDialog();
            });
            return bd;
        }
        public static BindingSelector ShowBindingSelector(string database, bool dbEnabled = true)
        {
            BindingSelector bd = new(database, dbEnabled);
            bd.ShowDialog();
            return bd;
        }
        public static BindingSelector ShowBindingSelector(string database, string table, bool dbEnabled = true, bool tableEnabled = true)
        {
            BindingSelector bd = new(database, table, dbEnabled, tableEnabled);
            Application.Current.Dispatcher.Invoke(() =>
            {
                bd.ShowDialog();
            });
            return bd;
        }
        public static bool ShowActiveUserSelector(out Session session, string helpText)
        {
            ActiveUserSelector au = new(helpText);
            Application.Current.Dispatcher.Invoke(() =>
            {
                au.ShowDialog();
            });
            if (au.SelectedSession != null)
            {
                session = au.SelectedSession;
                return true;
            }
            session = null;
            return false;
        }
        public static bool ShowUserSelector(out User user)
        {
            UserSelector us = new();
            Application.Current.Dispatcher.Invoke(() =>
            {
                us.ShowDialog();
            });
            if (us.Result == DialogStatus.Yes)
            {
                user = us.SelectedUser;
                return true;
            }
            user = null;
            return false;
        }
        public static Template ShowTemplateSelector(TemplateType type, string help)
        {
            TemplateSelector ts = new(type, help);
            Application.Current.Dispatcher.Invoke(() =>
            {
                ts.ShowDialog();
            });
            return ts.SelectedTemplate.AsModel();
        }
        public static void ShowWaitCursor(bool wait = true)
        {
            if (wait)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            }
            else
            {
                Mouse.OverrideCursor = null;
            }
        }
        #endregion

        #region Sector Managing
        public static void SetSectorByUser(User user)
        {
            string result = $"{CommonPath}\\{user.sector}.dbinc";
            if (File.Exists(result))
            {
                DatabasePath = result;
                using Sector s = new();
                s.slug = user.sector;
                s.GetSector();
                CurrentSector = s;
            }
        }
        #endregion

        public static async void ClearRuntimeFiles()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (string item in Directory.GetFiles(ProgramState.TemplatesRuntime))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            });
        }
        public static void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", path);
        }
        private static async void RemoveFilesOlderThan(string folder, DateTime time)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (string item in Directory.GetFiles(folder))
                {
                    if (File.GetCreationTime(item) < time)
                    {
                        try
                        {
                            File.Delete(item);
                        }
                        catch { }
                    }
                }
            });
        }
        public static void CollectGarbage()
        {
            if (LastGarbageCollect < DateTime.Now.AddMinutes(-5))
            {
                RemoveFilesOlderThan(ServerProcesses, DateTime.Now.AddHours(-8));
                RemoveFilesOlderThan(Exchanges, DateTime.Now.AddHours(-1));
                RemoveFilesOlderThan(TemplatesRuntime, DateTime.Now.AddHours(-1));
                RemoveFilesOlderThan(LogData, DateTime.Now.AddHours(-8));
                LastGarbageCollect = DateTime.Now;
            }
        }
    }
}
