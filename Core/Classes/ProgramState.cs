using Incas.Core.AutoUI;
using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.Core.Views.Windows;
using Incas.Objects.ViewModels;
using Incas.Users.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
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

        #region Server
        internal const int DefaultPort = 6890;
        internal const string EndMarker = "$[ENDMARKER]$";

        #endregion

        #region User
        internal static User CurrentUser { get; set; }
        internal static UserParameters CurrentUserParameters { get; set; }
        #endregion
        internal static MainWindowViewModel MainWindowViewModel { get; set; }
        internal static MainWindow MainWindow { get; set; }
        internal static CustomDatabaseViewModel DatabasePage { get; set; }
        internal static Workspace CurrentWorkspace { get; set; }

        internal static void InitDocumentsFolder()
        {
            Directory.CreateDirectory(WorkspacePaths.UserPathLicense);
            Directory.CreateDirectory(WorkspacePaths.UserPathLogs);
        }

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
        internal static string GetFullPathOfCustomDb(string path)
        {
            return $"{ProgramState.CurrentWorkspace.ObjectsPath}\\{path}.db";
        }
        internal static void ClearDataForRestart()
        {
            CurrentUser = null;
            Permission.CurrentUserPermission = PermissionGroup.Operator;
        }
        internal static bool CheckSensitive()
        {
            return CurrentUser != null;
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
                WorkspacePaths.SetCommonPath(RegistryData.GetSelectedWorkspacePath());
                return true;
            }
            return false;
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
            WorkspacePaths.SetCommonPath(data.Path, false);
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

        internal static void ClearRuntimeFiles()
        {
            ProgramState.CurrentWorkspace.ClearRuntimeFiles();
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
        internal static void CollectGarbage()
        {
            ProgramState.CurrentWorkspace.CollectGarbage();
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
        public static string ToHexString(string text)
        {
            byte[] b = Encoding.UTF8.GetBytes(text);
            return Convert.ToHexString(b);
        }
        public static string FromHexString(string hexString)
        {
            byte[] b = Convert.FromHexString(hexString);
            return System.Text.Encoding.UTF8.GetString(b);
        }
        internal static void UpdateWindowTabs()
        {
            MainWindow.UpdateTabs();
        }
        private static string GenerateUMI()
        {
            string key = Cryptographer.GenerateKey(Environment.MachineName);
            Guid id = Guid.NewGuid();
            return Cryptographer.EncryptString(key, id.ToString());
        }
        internal static void InitializeUMI()
        {
            RegistryData.SetUMI(ProgramState.GenerateUMI());
        }
        internal static string GetUMI()
        {
            string source = RegistryData.GetUMI();
            string key = Cryptographer.GenerateKey(Environment.MachineName);
            return Cryptographer.DecryptString(key, source);
        }
        internal static void CheckUMIExists()
        {
            if (string.IsNullOrEmpty(RegistryData.GetUMI()))
            {
                ProgramState.InitializeUMI();
            }
        }
        internal static bool CheckLicense()
        {
            string path = RegistryData.GetPathToLicense();
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            else
            {
                if (File.Exists(path))
                {
                    License.License l = License.License.ReadLicense(path);
                    if (!l.CheckVerificationSignatures())
                    {
                        DialogsManager.ShowAccessErrorDialog($"Лицензия на данное ПО скомпрометирована и не является подлинной. Пожалуйста, укажите правильную лицензию в следующем окне.", "Лицензия невалидна");
                        return false;
                    }
                    else
                    {
                        if (l.IsActual())
                        {
                            if (l.IsItForMe())
                            {                               
                                return true;
                            }
                            else
                            {
                                DialogsManager.ShowAccessErrorDialog($"Данная лицензия не подходит к данному устройству.", "Лицензия невалидна");
                                return false;
                            } 
                        }
                        else
                        {
                            DialogsManager.ShowAccessErrorDialog($"Предельный срок действия лицензии истек. Дальнейшее использование программы станет возможным только после прикрепления новой лицензии.", "Лицензия истекла");
                            return false;
                        }                       
                    }
                }
                else
                {
                    DialogsManager.ShowAccessErrorDialog($"По указанному пути ({path}) лицензия не найдена.", "Лицензия не найдена");
                    return false;
                }
            }
        }
    }
}
