using Common;
using Incas.Core.AutoUI;
using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.Core.Views.Windows;
using Incas.Users.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;

namespace Incas.Core.Classes
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

    internal static class ProgramState
    {
        public static string CommonPath { get; private set; }
        public static string UserPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Incas";
        public static string DatabasePath { get; private set; }
        public static string CustomDatabasePath => Root + @"\Databases";
        public static string ServiceDatabasePath => Root + @"\service.dbinc";
        public static string Root => CommonPath + @"\Root";
        public static string ServerProcesses => Root + @"\ServerProcesses";  // ...\Root\ServerProccesses
        public static string Exchanges => Root + @"\Exchanges";  // ...\Root\Exchanges
        public static string Messages => Root + @"\Messages";  // папка еще не создана

        public static string Scripts => Root + @"\Scripts";
        public static string LogData => Root + @"\LogData";  // ...\Root\LogData

        #region Templates

        private static string Templates => Root + @"\Templates";     // ...\Root\Templates
        public static string TemplatesSourcesWordPath => Templates + @"\Sources\Word";     // ...\Root\Templates\Sources\Word
        public static string TemplatesSourcesExcelPath => Templates + @"\Sources\Excel";     // ...\Root\Templates\Sources\Excel
        public static string TemplatesRuntime => Templates + @"\Runtime";     // ...\Root\Templates\Runtime
        #endregion

        #region User
        public static User CurrentUser { get; set; }
        public static UserParameters CurrentUserParameters { get; set; }
        public static Session CurrentSession { get; private set; }
        #endregion
        public static MainWindowViewModel MainWindow { get; set; }

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
            //ScriptManager.Execute("from Incas import Service", ScriptManager.GetEngine().CreateScope());
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
            return CurrentUser != null && CurrentSession != null;
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
                Dialog q = new("По указанному пути рабочее пространство не обнаружено.\n" +
                "Проверьте правильность введенного пути или создайте новое рабочее пространство.", "Рабочее пространство не обнаружено");
                q.ShowDialog();
            }
        }
        #endregion

        #region ComputerId
        public static string GenerateSlug(int len, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            Random random = new();
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
            Parameter par = new();
            par.GetParameter(type, name, defaultValue, createIfNot);
            return par;
        }

        #region Incubator
        public static void InitWorkspace(CreateWorkspace data)
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
                        par.value = data.WorkspaceName;
                        par.CreateParameter();
                    }

                    DialogsManager.ShowInfoDialog("Рабочее пространство успешно создано.");

                    using (User user = new())
                    {
                        user.username = "admin";
                        user.post = "Администратор рабочего пространства";
                        user.surname = "Администратор";
                        user.fullname = "Администратор";
                        UserParameters up = new()
                        {
                            permission_group = PermissionGroup.Admin,
                            password = data.Password,
                        };
                        user.GenerateSign();
                        user.SaveParametersContext(up);
                        user.AddUser();
                    }                  
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
                        DialogsManager.ShowErrorDialog("Нечто блокирует файл базы данных.");
                    }
                }
            }

            DialogsManager.ShowWaitCursor();
            Permission.CurrentUserPermission = PermissionGroup.Admin;
            SetCommonPath(data.Path, false);
            if (CreateTablesInDatabase())
            {
                DialogsManager.ShowWaitCursor(false);
                RegistryData.SetWorkspacePath(data.WorkspaceName, data.Path);
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
                    DialogsManager.ShowErrorDialog("Действия по добавлению, изменению " +
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
            using Session ms = new();
            ms.AddSession();
            CurrentSession = ms;
            ms.ClearOldestSessions();
            //Directory.CreateDirectory(ServerProcessor.Port);
        }
        public static List<Session> GetActiveSessions()
        {
            using Session ms = new();
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
                SoundPlayer myNewSound = new(stream);
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
                DialogsManager.ShowErrorDialog($"Не удалось открыть присланную ссылку:\n{ex.Message}", "Действие невозможно");
            }
        }

        public static async void ClearRuntimeFiles()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (string item in Directory.GetFiles(TemplatesRuntime))
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
        public static string GetConstant(string name)
        {
            using (Parameter p = new())
            {
                return p.GetConstantValue(name);
            }
        }
        public static List<string> GetEnumeration(string name)
        {
            using (Parameter p = new())
            {
                return p.GetEnumerationValue(name);
            }
        }
    }
}
