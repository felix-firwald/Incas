using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    static class RegistryData
    {
        public static RegistryKey GetRoot()
        {
            return Registry.CurrentUser.OpenSubKey("Incubator", true);
        }
        public static RegistryKey GetTemplatesData() 
        {
            return GetRoot().OpenSubKey("TemplatesData", true);
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
                return GetTemplatesData().OpenSubKey(name);
            }
            AddTemplate(name, "", "", "");
            return GetTemplatesData().OpenSubKey(name);
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
    }
}
