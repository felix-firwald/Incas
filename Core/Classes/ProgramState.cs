using Incas.Core.AutoUI;
using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.Core.Views.Windows;
using Incas.Objects.Components;
using Incas.Objects.ViewModels;
using Incas.Users.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;

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
        internal static string CommonPath { get; private set; }
        internal static string UserPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Incas";
        internal static string ObjectsPath => Root + @"\Objects";
        internal static string ServiceDatabasePath => CommonPath + @"\service.incas";
        internal static string Root => CommonPath + @"\Root";
        internal static string ServerProcesses => Root + @"\ServerProcesses";  // ...\Root\ServerProccesses
        internal static string Exchanges => Root + @"\Exchanges";  // ...\Root\Exchanges
        internal static string Messages => Root + @"\Messages";  // папка еще не создана

        internal static string Scripts => Root + @"\Scripts";
        internal static string LogData => Root + @"\LogData";  // ...\Root\LogData
        #region Server
        internal const int DefaultPort = 6890;
        internal const string EndMarker = "$[ENDMARKER]$";
        #endregion
        #region Templates

        private static string Templates => Root + @"\Templates";     // ...\Root\Templates
        public static string TemplatesSources => Templates + @"\Sources";
        public static string TemplatesRuntime => Templates + @"\Runtime";     // ...\Root\Templates\Runtime
        #endregion

        #region User
        internal static User CurrentUser { get; set; }
        internal static UserParameters CurrentUserParameters { get; set; }
        internal static Session CurrentSession { get; private set; }
        #endregion
        internal static MainWindowViewModel MainWindowViewModel { get; set; }
        internal static MainWindow MainWindow { get; set; }
        internal static CustomDatabaseViewModel DatabasePage { get; set; }

        internal static IPAddress GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return IPAddress.None;
        }
        internal static IPAddress GetGlobalIPAddress()
        {
            IPAddress ipAddress;
            const string url = "https://api.ipify.org";
            try
            {
                using (HttpClient client = new())
                {
                    string stringIp = client.GetStringAsync(url).GetAwaiter().GetResult();
                    DialogsManager.ShowInfoDialog("!");
                    ipAddress = IPAddress.Parse(stringIp);
                }        
            }
            catch
            {
                return IPAddress.None;
            }          
            return ipAddress;
        }

        internal static void CheckoutWorkspaces()
        {
            List<string> workspaces = RegistryData.GetWorkspaces();
            if (!workspaces.Contains(RegistryData.GetSelectedWorkspace()))
            {
                RegistryData.SetSelectedWorkspace("");
            }
            foreach (string ws in workspaces)
            {
                string path = RegistryData.GetWorkspacePath(ws);
                if (string.IsNullOrEmpty(path) || !File.Exists(path + @"\service.incas"))
                {
                    DialogsManager.ShowErrorDialog($"Рабочее пространство, записанное под именем \"{ws}\", " +
                        $"повреждено или не существует по пути:\n\"{path}\", в связи с чем оно будет удалено из списка доступных рабочих пространств.");
                    RegistryData.RemoveWorkspace(ws);
                    if (RegistryData.GetSelectedWorkspace() == ws)
                    {
                        RegistryData.SetSelectedWorkspace("");
                    }
                }
            }
        }

        private static DateTime LastGarbageCollect = DateTime.Now;
        #region Path and init
        internal static void SetCommonPath(string path, bool checkout = true)
        {
            CommonPath = path;
            Directory.CreateDirectory(Templates);
            Directory.CreateDirectory(ServerProcesses);
            Directory.CreateDirectory(Scripts);
            Directory.CreateDirectory(Exchanges);
            Directory.CreateDirectory(ObjectsPath);
            Directory.CreateDirectory(Messages);
            Directory.CreateDirectory(LogData);
            Directory.CreateDirectory(TemplatesRuntime);
            Directory.CreateDirectory(TemplatesSources);
            if (checkout)
            {
                DatabaseManager.ActualizeTables();
            }
            CollectGarbage();
        }
        internal static string GetFullPathOfCustomDb(string path)
        {
            return $"{ObjectsPath}\\{path}.db";
        }
        internal static void ClearDataForRestart()
        {
            CurrentUser = null;
            CurrentSession = null;
            CommonPath = null;
            Permission.CurrentUserPermission = PermissionGroup.Operator;
        }
        internal static bool CheckSensitive()
        {
            return CurrentUser != null && CurrentSession != null;
        }
        internal static bool IsCommonPathExists()
        {
            return Directory.Exists(CommonPath);
        }
        #endregion

        #region ComputerId
        internal static string GenerateSlug(int len, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
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
        internal static bool LoadUserData()
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
        internal static string GetFullnameOfDocumentFile(string name)
        {
            return TemplatesSources + "\\" + name;
        }
        #endregion
        private static bool CreateTablesInDatabase()
        {
            return DatabaseManager.InitializeService();
        }
        internal static Parameter GetParameter(ParameterType type, string name, string defaultValue = "0", bool createIfNot = true)
        {
            Parameter par = new();
            par.GetParameter(type, name, defaultValue, createIfNot);
            return par;
        }

        #region Incubator
        internal static void InitWorkspace(CreateWorkspace data)
        {
            int recursion = 0;
            void Create()
            {
                try
                {
                    recursion++;
                    WorkspacePrimarySettings wps = new()
                    {
                        Name = data.WorkspaceName,
                        IsLocked = false
                    };
                    using (Parameter par = new())
                    {
                        par.type = ParameterType.WORKSPACE;
                        par.name = "ws_data";
                        par.value = JsonConvert.SerializeObject(wps);
                        par.CreateParameter();
                    }

                    DialogsManager.ShowInfoDialog("Рабочее пространство успешно создано.");

                    using User user = new();
                    user.username = "admin";
                    user.post = "Администратор рабочего пространства";
                    user.surname = "Администратор";
                    user.fullname = "Администратор";
                    UserParameters up = new()
                    {
                        Permission_group = PermissionGroup.Admin,
                        Password = data.Password,
                    };
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
                Create();
                RegistryData.SetWorkspacePath(data.WorkspaceName, data.Path);
            }
        }
        private static WorkspacePrimarySettings GetWorkspaceSettings()
        {
            using Parameter par = GetParameter(ParameterType.WORKSPACE, "ws_data", "", false);
            try
            {
                return JsonConvert.DeserializeObject<WorkspacePrimarySettings>(par.value);
            }
            catch
            {
                DialogsManager.ShowErrorDialog("Первичные данные о рабочем пространстве повреждены.", "Критическая ошибка");
                return new();
            }
        }
        internal static string GetWorkspaceName()
        {
            return GetWorkspaceSettings().Name;
        }
        internal static bool IsWorkspaceLocked()
        {
            return GetWorkspaceSettings().IsLocked;
        }

        #endregion
        internal static void CheckLocked()
        {
            if (IsWorkspaceLocked() && Permission.CurrentUserPermission != PermissionGroup.Admin)
            {
                throw new LockedException();
            }
        }

        #region Session

        internal static void BrokeSession()
        {
            throw new SessionBrokenException();
        }
        internal static void OpenSession()
        {
            using Session ms = new();
            ms.AddSession();
            CurrentSession = ms;
            ms.ClearOldestSessions();
        }
        internal static List<Session> GetActiveSessions()
        {
            using Session ms = new();
            return ms.GetOpenedSessions();
        }
        internal static void CloseSession()
        {
            if (CurrentSession != null && CurrentSession.active)
            {
                CurrentSession.CloseSession();
            }
            ServerProcessor.StopPort();
        }
        #endregion
        internal static void PlaySound(string path)
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
        internal static void OpenWebPage(string url)
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

        internal static async void ClearRuntimeFiles()
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
        internal static void OpenFolder(string path)
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
        internal async static void CollectGarbage()
        {
            await Task.Run(() =>
            {               
                if (LastGarbageCollect < DateTime.Now.AddMinutes(-5))
                {
                    ProgramStatusBar.SetText("Выполняется сборка мусора...");
                    RemoveFilesOlderThan(ServerProcesses, DateTime.Now.AddHours(-8));
                    RemoveFilesOlderThan(Exchanges, DateTime.Now.AddHours(-1));
                    RemoveFilesOlderThan(TemplatesRuntime, DateTime.Now.AddHours(-1));
                    RemoveFilesOlderThan(LogData, DateTime.Now.AddHours(-8));
                    ObjectProcessor.CleanGarbage();
                    LastGarbageCollect = DateTime.Now;
                    ProgramStatusBar.Hide();
                }               
            });
            
        }
        internal static string GetConstant(string name)
        {
            using Parameter p = new();
            return p.GetConstantValue(name);
        }
        internal static string GetConstant(Guid id)
        {
            using Parameter p = new();
            return p.GetConstantValue(id);
        }
        internal static List<string> GetEnumeration(Guid id)
        {
            using Parameter p = new();
            return p.GetEnumerationValue(id);
        }
    }
}
