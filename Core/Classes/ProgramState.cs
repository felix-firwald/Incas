using Incas.Core.AutoUI;
using Incas.Core.ViewModels;
using Incas.Core.Views.Windows;
using Incas.Objects.ViewModels;
using IncasEngine.Core;
using IncasEngine.Models;
using IncasEngine.Workspace;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

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

        
        internal static MainWindowViewModel MainWindowViewModel { get; set; }
        internal static MainWindow MainWindow { get; set; }
        internal static CustomDatabaseViewModel DatabasePage { get; set; }
        internal static Workspace CurrentWorkspace
        {
            get
            {
                return IncasEngine.Core.EngineGlobals.CurrentWorkspace;
            }
            set
            {
                IncasEngine.Core.EngineGlobals.CurrentWorkspace = value;
            }
        }
        static ProgramState()
        {
            IncasEngine.Core.EngineEvents.OnUpdateAllRequested += EngineEvents_OnUpdateAllRequested;
        }

        private static void EngineEvents_OnUpdateAllRequested()
        {
            MainWindowViewModel.LoadInfo();
            MainWindow.UpdateTabs();
        }

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
        internal static bool CheckSensitive()
        {
            return CurrentWorkspace != null;
        }
        #endregion

        #region ComputerId
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
        

        #region Incubator
        internal static void InitWorkspace(CreateWorkspace data)
        {
            DialogsManager.ShowWaitCursor();
            ProgramState.CurrentWorkspace = new(data.Path);
            ProgramState.CurrentWorkspace.InitializeDefinition(data);
            RegistryData.SetWorkspacePath(data.WorkspaceName, data.Path);
            DialogsManager.ShowWaitCursor(false);
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

        internal static void ClearRuntimeFiles()
        {
            ProgramState.CurrentWorkspace.ClearRuntimeFiles();
        }
        internal static void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", path);
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
    }
}
