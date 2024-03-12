using Common;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Incubator_2.Common
{
    public enum RestrictionType // потому что эта тупая хуета не десериализует
    {
        WriteEdit,
        EditOnly,
        WriteOnly,
        ReadOnly,
        NoAccess
    }

    public class UserParameters
    {
        public bool tasks_visibility { get; set; }
        public bool communication_visibility { get; set; }
        public bool database_visibility { get; set; }

        public bool create_templates { get; set; }
        public bool modify_templates { get; set; }
        public string startup_password { get; set; }
        public string password { get; set; }
        public PermissionGroup permission_group { get; set; }
        public bool write_log { get; set; }
        public bool write_listen_log { get; set; }

        public bool IsRightPassword(string input)
        {
            if (string.IsNullOrEmpty(password))
            {
                return input == startup_password;
            }
            return input == password;
        }
        public void Show()
        {
            ProgramState.ShowInfoDialog($"startup_password: {startup_password}, password: {password}");
        }

        public void ApplyStandartProperties(string pwd)
        {
            tasks_visibility = true;
            communication_visibility = true;
            database_visibility = false;
            create_templates = false;
            modify_templates = false;
            startup_password = pwd;
            permission_group = PermissionGroup.Operator;
        }
        public void ApplyAdminProperties(string pwd)
        {
            tasks_visibility = true;
            communication_visibility = true;
            database_visibility = true;
            create_templates = true;
            modify_templates = true;
            startup_password = pwd;
            permission_group = PermissionGroup.Admin;
        }

    }
}
    
