using Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using Incubator_2.Windows;
using Incubator_2;
using System.Windows;
using Incubator_2.Models;
using System.Runtime.CompilerServices;
using Incubator_2.Common;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Input;
using Incubator_2.Windows.CustomDatabase;
using Incubator_2.ViewModels;


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
    struct FirstWorkspaceData
    {
        public string workspacePath;
        public string workspaceName;
        public string userSurname;
        public string userFullname;
        public string userPassword;
    }

    static class ProgramState
    {
        public static string CommonPath { get; private set; }
        public static string UserPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Incubator";
        public static string DatabasePath { get; private set; }
        public static string CustomDatabasePath { get { return Root + @"\Databases"; } }
        public static string ServiceDatabasePath { get { return Root + @"\service.dbinc"; } }
        public static string Root { get { return CommonPath + @"\Root"; } }
        public static string ServerProcesses { get { return Root + @"\ServerProcesses"; } } // ...\Root\ServerProccesses
        public static string Exchanges { get { return Root + @"\Exchanges"; } } // ...\Root\Exchanges
        public static string Messages { get { return Root + @"\Messages"; } } // папка еще не создана

        public static string UsersContext { get { return Root + @"\UsersContext"; } }
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
        public async static void SetCommonPath(string path)
        {
            CommonPath = path;
            DatabasePath = path + @"\data.dbinc";
            await System.Threading.Tasks.Task.Run(() =>
            {
                Directory.CreateDirectory(Templates);
                Directory.CreateDirectory(TemplatesSourcesWordPath);
                Directory.CreateDirectory(TemplatesSourcesExcelPath);
                Directory.CreateDirectory(ServerProcesses);
                Directory.CreateDirectory(Exchanges);
                Directory.CreateDirectory(CustomDatabasePath);
                Directory.CreateDirectory(Messages);
                Directory.CreateDirectory(LogData);
                Directory.CreateDirectory(TemplatesRuntime);
            });
            CollectGarbage();
            
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
        #endregion
        private static bool CreateTablesInDatabase()
        {
            return DatabaseManager.InitializeService();
        }
        public static Parameter GetParameter(ParameterType type, string name, string defaultValue="0", bool createIfNot = true)
        {
            Parameter par = new Parameter();
            par.GetParameter(type, name, defaultValue, createIfNot);
            return par;
        }

        #region Incubator
        public static void InitWorkspace(FirstWorkspaceData data)
        {
            Permission.CurrentUserPermission = PermissionGroup.Admin;
            SetCommonPath(data.workspacePath);
            if (CreateTablesInDatabase())
            {
                RegistryData.SetWorkspacePath(data.workspaceName, data.workspacePath);
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
                using (Sector sector = new())
                {
                    sector.slug = "data";
                    sector.name = "Базовый сектор";
                    sector.AddSector(false);
                }
                using (User user = new())
                {
                    user.username = "admin";
                    user.post = "Администратор рабочего пространства";
                    user.surname = data.userSurname;
                    user.secondName = data.userFullname;
                    user.fullname = $"{user.surname} {user.secondName}";
                    user.sector = "data";
                    user.AddUser();
                    UserParameters up = new();
                    up.permission_group = PermissionGroup.Admin;
                    up.tasks_visibility = true;
                    up.communication_visibility = true;
                    up.database_visibility = true;
                    up.password = data.userPassword;
                    UserContextor.SaveContext(up, user);
                }
            }
        }
        public static string GetWorkspaceName()
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_name", "Рабочая область"))
            {
                return par.value;
            }
        }
        public static void SetWorkspaceName(string name)
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_name", "Рабочая область"))
            {
                par.value = name;
                par.UpdateValue();
            }
        }
        public static void SetWorkspaceOpened(bool opened)
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_opened", "1"))
            {
                par.WriteBoolValue(opened);
                par.UpdateValue();
            }
        }
        public static bool IsWorkspaceOpened()
        {
            if (Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_opened", "1"))
                {
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
            }
            else
            {
                return true;
            }
        }
        public static void SetWorkspaceLocked(bool locked)
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_locked"))
            {
                par.WriteBoolValue(locked);
                par.UpdateValue();
            } 
        }
        public static bool IsWorkspaceLocked()
        {
            using (Parameter par = GetParameter(ParameterType.INCUBATOR, "ws_locked"))
            {
                return par.GetValueAsBool();
            }
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
            using (Session ms = new Session())
            {
                ms.AddSession();
                CurrentSession = ms;
                ms.ClearOldestSessions();
            }
            //Directory.CreateDirectory(ServerProcessor.Port);
        }
        public static List<Session> GetActiveSessions()
        {
            using (Session ms = new Session())
            {
                return ms.GetOpenedSessions();
            }
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

        #region Modal Dialogs
        public static void ShowErrorDialog(string message, string title = "Возникла неизвестная ошибка")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.Error);
                d.ShowDialog();
            });
        }
        public static void ShowDatabaseErrorDialog(string message, string title = "Ошибка при выполнении запроса")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.DatabaseError);
                d.ShowDialog();
            });
        }
        public static void ShowAccessErrorDialog(string message, string title = "Нет доступа")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.AccessDenied);
                d.ShowDialog();
            });
        }
        public static void ShowExclamationDialog(string message, string title = "Обратите внимание")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.Exclamation);
            d.ShowDialog();
            });
        }
        public static void ShowInfoDialog(string message, string title = "Оповещение")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Dialog d = new Dialog(message, title, Dialog.DialogIcon.Info);
                d.ShowDialog();
            });
        }
        public static DialogStatus ShowQuestionDialog(string message, string title, string yesText="Да", string noText = "Нет")
        {
            DialogQuestion d = new DialogQuestion(message, title, yesText, noText);
            Application.Current.Dispatcher.Invoke(() =>
            {
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
        public static Session ShowActiveUserSelector(string helpText)
        {
            ActiveUserSelector au = new(helpText);
            Application.Current.Dispatcher.Invoke(() =>
            {
                au.ShowDialog();
            });
            if (au.SelectedSession != null)
            {
                return au.SelectedSession;
            }
            else
            {
                return new();
            }
        }
        public static User ShowUserSelector()
        {
            UserSelector us = new();
            Application.Current.Dispatcher.Invoke(() =>
            {
                us.ShowDialog();
            });
            if (us.Result == DialogStatus.Yes)
            {
                return us.SelectedUser;
            }
            return new();
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
                using (Sector s = new())
                {
                    s.slug = user.sector;
                    s.GetSector();
                    CurrentSector = s;
                }
            }
        }
        #endregion

        public async static void ClearRuntimeFiles()
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
        private async static void RemoveFilesOlderThan(string folder, DateTime time)
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
