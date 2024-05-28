using Common;
using Incas.Core.ViewModels;

namespace Incas.ViewModels
{
    internal class VM_AdminPanel : BaseViewModel
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
        public bool IsAdmin => Permission.CurrentUserPermission is PermissionGroup.Admin;
    }
}
