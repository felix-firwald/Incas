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
        public string Startup_password { get; set; }
        public string Password { get; set; }
        public PermissionGroup Permission_group { get; set; }
        public string Group { get; set; }

        public bool IsRightPassword(string input)
        {
            return string.IsNullOrEmpty(this.Password) ? input == this.Startup_password : input == this.Password;
        }

        public void ApplyStandartProperties(string pwd)
        {
            this.Startup_password = pwd;
            this.Permission_group = PermissionGroup.Operator;
        }
        public void ApplyAdminProperties(string pwd)
        {
            this.Startup_password = pwd;
            this.Permission_group = PermissionGroup.Admin;
        }
    }
}

