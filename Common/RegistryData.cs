using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CA1416 // Проверка совместимости платформы
namespace Common
{
    internal struct FilesManagerSetting
    {
        public string preffered_path = "";
        public bool filter_start_cb = false;
        public string filter_start_tb = "";
        public bool filter_end_cb = false;

        public bool filter_ignore_extension_cb = true;
        public string filter_end_tb = "";
        public bool filter_contains_cb = false;
        public string filter_contains_tb = "";
        public bool subfolders_cb = false;

        public FilesManagerSetting()
        {

        }
    }

    internal static class RegistryData
    {
        public static RegistryKey GetRoot()
        {
            return Registry.CurrentUser.CreateSubKey("Incubator", true);
        }
        public static string GetComputer()
        {
            string key = "computer";
            if (GetRoot().GetValueNames().Contains(key))
            {
                return GetRoot().GetValue(key).ToString();
            }
            string slug = ProgramState.GenerateSlug(20);
            GetRoot().SetValue(key, slug);
            return slug;
        }
        #region workspaces

        public static RegistryKey GetWorkspaceData()
        {
            return GetRoot().CreateSubKey("Workspaces", true);
        }
        public static List<string> GetWorkspaces()
        {
            return GetWorkspaceData().GetSubKeyNames().ToList();
        }
        public static string GetSelectedWorkspace()
        {
            return GetRoot().GetValue("selected_workspace", "").ToString();
        }
        public static void SetSelectedWorkspace(string selected)
        {
            GetRoot().SetValue("selected_workspace", selected);
        }

        public static string GetSelectedWorkspacePath()
        {
            return GetWorkspacePath(GetSelectedWorkspace());
        }
        public static bool IsWorkspaceExists(string workspace)
        {
            return GetWorkspaceData().GetSubKeyNames().Contains(workspace);
        }
        public static bool IsRoosterExists()
        {
            return GetRoot().GetValueNames().Contains("Rooster");
        }

        public static RegistryKey GetWorkspaceByName(string name)
        {
            if (GetWorkspaceData().GetSubKeyNames().Contains(name))
            {
                return GetWorkspaceData().OpenSubKey(name, true);
            }
            RegistryKey rk = GetWorkspaceData().CreateSubKey(name, true);
            rk.SetValue("path", "");
            rk.SetValue("password", "");
            return rk;
        }
        public static void RemoveWorkspace(string name)
        {
            GetWorkspaceData().DeleteSubKeyTree(name, true);
        }
        public static string GetWorkspacePath(string name)
        {
            return GetWorkspaceByName(name).GetValue("path").ToString();
        }
        public static void SetWorkspacePath(string name, string path)
        {
            GetWorkspaceByName(name).SetValue("path", path);
        }
        public static string GetWorkspaceSelectedUser(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }

            return GetWorkspaceByName(name).GetValue("user", "").ToString();
        }
        public static void SetWorkspaceSelectedUser(string name, string user)
        {
            GetWorkspaceByName(name).SetValue("user", user);
        }
        public static string GetWorkspacePassword(string name)
        {
            return GetWorkspaceByName(name).GetValue("password", "").ToString();
        }
        public static void SetWorkspacePassword(string name, string password)
        {
            GetWorkspaceByName(name).SetValue("password", password);
        }

        #endregion
        #region Templates
        public static RegistryKey GetTemplatesData()
        {
            return GetWorkspaceByName(GetSelectedWorkspace()).CreateSubKey("TemplatesData", true);

        }

        public static RegistryKey AddTemplate(string name, string prPath, string prPrefix, string prPostfix)
        {
            RegistryKey rk = GetTemplatesData().CreateSubKey(name, true);
            rk.SetValue("preferred_path", prPath);
            rk.SetValue("preferred_prefix", prPrefix);
            rk.SetValue("preferred_postfix", prPostfix);
            rk.Close();
            return rk;
        }
        private static RegistryKey GetTemplate(string name)
        {
            if (GetTemplatesData().GetSubKeyNames().Contains(name))
            {
                return GetTemplatesData().OpenSubKey(name, true);
            }
            AddTemplate(name, "", "", "");
            return GetTemplatesData().OpenSubKey(name, true);
        }
        public static string GetTemplatePreferredPath(string template)
        {
            return GetTemplate(template).GetValue("preferred_path").ToString();
        }
        public static string GetTemplatePreferredPrefix(string template)
        {
            return GetTemplate(template).GetValue("preferred_prefix").ToString();
        }
        public static string GetTemplatePreferredPostfix(string template)
        {
            return GetTemplate(template).GetValue("preferred_postfix").ToString();
        }
        public static void SetTemplatePreferredPath(string template, string path)
        {
            GetTemplate(template).SetValue("preferred_path", path);
        }
        public static void SetTemplatePreferredPrefix(string template, string prefix)
        {
            GetTemplate(template).SetValue("preferred_prefix", prefix);
        }
        public static void SetTemplatePreferredPostfix(string template, string postfix)
        {
            GetTemplate(template).SetValue("preferred_postfix", postfix);
        }
        #endregion

        private static RegistryKey GetTools()
        {
            return GetRoot().CreateSubKey("Tools", true);
        }
        #region FilesManager
        private static RegistryKey GetFilesManager()
        {
            return GetTools().CreateSubKey("FilesManager", true);
        }
        public static string[] GetSettingsOfFM()
        {
            return GetFilesManager().GetSubKeyNames();
        }
        public static void SaveSettingForFM(string name, FilesManagerSetting setting)
        {
            RegistryKey rk = GetFilesManager().CreateSubKey(name, true);
            rk.SetValue("preffered_path", setting.preffered_path);
            rk.SetValue("filter_start_cb", setting.filter_start_cb);
            rk.SetValue("filter_start_tb", setting.filter_start_tb);
            rk.SetValue("filter_end_cb", setting.filter_end_cb);

            rk.SetValue("filter_ignore_extension_cb", setting.filter_ignore_extension_cb);
            rk.SetValue("filter_end_tb", setting.filter_end_tb);
            rk.SetValue("filter_contains_cb", setting.filter_contains_cb);
            rk.SetValue("filter_contains_tb", setting.filter_contains_tb);

            rk.SetValue("subfolders_cb", setting.subfolders_cb);
            rk.Close();
        }
        public static FilesManagerSetting GetSettingByNameFM(string name)
        {
            FilesManagerSetting setting = new FilesManagerSetting();
            return setting;
        }
        #endregion
        private static RegistryKey GetDocumentsEditor()
        {
            return GetTools().CreateSubKey("DocumentsEditor", true);
        }
    }
}
#pragma warning restore CA1416 // Проверка совместимости платформы