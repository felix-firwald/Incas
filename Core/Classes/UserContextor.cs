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
        public string startup_password { get; set; }
        public string password { get; set; }
        public PermissionGroup permission_group { get; set; }

        public bool IsRightPassword(string input)
        {
            return string.IsNullOrEmpty(this.password) ? input == this.startup_password : input == this.password;
        }

        public void ApplyStandartProperties(string pwd)
        {
            this.startup_password = pwd;
            this.permission_group = PermissionGroup.Operator;
        }
        public void ApplyAdminProperties(string pwd)
        {
            this.startup_password = pwd;
            this.permission_group = PermissionGroup.Admin;
        }
    }
}

