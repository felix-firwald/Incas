using Common;

namespace Incubator_2.ViewModels
{
    internal class VM_AdminPanel : VM_Base
    {
        public VM_AdminPanel() { }

        public string HeaderText
        {
            get
            {
                switch (Permission.CurrentUserPermission)
                {
                    case PermissionGroup.Admin:
                        return "Панель администратора";
                    case PermissionGroup.Moderator:
                        return "Панель модератора";
                    default:
                        return "Панель ошибок";
                }
            }
        }
        public bool IsAdmin
        {
            get { return Permission.CurrentUserPermission is PermissionGroup.Admin; }
        }
    }
}
