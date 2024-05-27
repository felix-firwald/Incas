using Common;
using Incas.Core.ViewModels;
using Incubator_2.Windows;
using Models;


namespace Incubator_2.ViewModels.VMAdmin
{
    internal class VM_UserElement : BaseViewModel
    {
        private readonly User _user;
        public VM_UserElement(User user)
        {
            this._user = user;
        }
        public string UserName
        {
            get
            {
                return this._user.fullname;
            }
            set
            {
                this.OnPropertyChanged(nameof(this.UserName));
            }
        }
        public string UserStatus
        {
            get
            {
                switch (this._user.GetParametersContext().permission_group)
                {
                    case PermissionGroup.Admin:
                        return "Администратор";
                    case PermissionGroup.Moderator:
                        return "Модератор";
                    case PermissionGroup.Editor:
                        return "Редактор";
                    case PermissionGroup.Operator:
                    default:
                        return "Оператор";
                }
            }
            set
            {
                this.OnPropertyChanged(nameof(this.UserStatus));
            }
        }
        public string UserPost
        {
            get
            {
                return this._user.post;
            }
            set
            {
                this.OnPropertyChanged(nameof(this.UserPost));
            }
        }
        public bool IsRemovable
        {
            get
            {
                if (this._user.id == 1 || this._user.username == "admin")
                {
                    return false;
                }
                return true;
            }
        }
        public void RemoveUser()
        {
            this._user.RemoveUser();
        }
        public void EditUser()
        {
            UserEditor editor = new(this._user);
            editor.ShowDialog();
            this.OnPropertyChanged(nameof(this.UserName));
            this.OnPropertyChanged(nameof(this.UserStatus));
            this.OnPropertyChanged(nameof(this.UserPost));
        }
    }
}
