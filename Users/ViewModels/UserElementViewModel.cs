using Common;
using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Users.AutoUI;
using Incas.Users.Models;
using Incas.Users.Views.Windows;

namespace Incas.Users.ViewModels
{
    internal class UserElementViewModel : BaseViewModel
    {
        private readonly User _user;
        public UserElementViewModel(User user)
        {
            this._user = user;
        }
        public string UserName
        {
            get => this._user.fullname;
            set => this.OnPropertyChanged(nameof(this.UserName));
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
            set => this.OnPropertyChanged(nameof(this.UserStatus));
        }
        public string UserPost
        {
            get => this._user.post;
            set => this.OnPropertyChanged(nameof(this.UserPost));
        }
        public bool IsRemovable => this._user.id != 1 && this._user.username != "admin";
        public void RemoveUser()
        {
            this._user.RemoveUser();
        }
        public void EditUser()
        {
            //UserEditor editor = new(this._user);
            //editor.ShowDialog();
            if (this._user.username == "admin" && this._user.id == 1)
            {
                UserSuperAdminSettings usa = new(this._user);
                DialogsManager.ShowSimpleFormDialog(usa, "Редактирование владельца", Icon.UserGears);
            }
            else
            {
                UserSettings us = new(this._user);
                DialogsManager.ShowSimpleFormDialog(us, "Редактирование пользователя", Icon.UserGears);
            }          
            this.OnPropertyChanged(nameof(this.UserName));
            this.OnPropertyChanged(nameof(this.UserStatus));
            this.OnPropertyChanged(nameof(this.UserPost));
        }
    }
}
