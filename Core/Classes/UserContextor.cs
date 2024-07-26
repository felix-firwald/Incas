using Common;

namespace Incas.Core.Classes
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
        //public bool tasks_visibility { get; set; }
        //public bool communication_visibility { get; set; }
        //public bool database_visibility { get; set; }

        //public bool create_templates { get; set; }
        //public bool modify_templates { get; set; }
        public string startup_password { get; set; }
        public string password { get; set; }
        public PermissionGroup permission_group { get; set; }
        //public bool write_log { get; set; }
        //public bool write_listen_log { get; set; }

        public bool IsRightPassword(string input)
        {
            return string.IsNullOrEmpty(this.password) ? input == this.startup_password : input == this.password;
        }
        //public void Show()
        //{
        //    ProgramState.ShowInfoDialog($"startup_password: {startup_password}, password: {password}, tasks_visibility: {tasks_visibility}");
        //}

        public void ApplyStandartProperties(string pwd)
        {
            //this.tasks_visibility = true;
            //this.communication_visibility = true;
            //this.database_visibility = false;
            //this.create_templates = false;
            //this.modify_templates = false;
            this.startup_password = pwd;
            this.permission_group = PermissionGroup.Operator;
        }
        public void ApplyAdminProperties(string pwd)
        {
            //this.tasks_visibility = true;
            //this.communication_visibility = true;
            //this.database_visibility = true;
            //this.create_templates = true;
            //this.modify_templates = true;
            this.startup_password = pwd;
            this.permission_group = PermissionGroup.Admin;
        }

    }
}

