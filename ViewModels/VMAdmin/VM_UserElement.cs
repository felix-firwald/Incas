using Models;
using Common;
using Incubator_2.Windows;


namespace Incubator_2.ViewModels.VMAdmin
{
    class VM_UserElement : VM_Base
    {
        private readonly User _user;
        public VM_UserElement(User user)
        {
            _user = user;
        }
        public string UserName
        {
            get
            {
                return $"{_user.surname} {_user.fullname}";
            }
            set
            {
                OnPropertyChanged(nameof(UserName));
            }
        }
        public string UserStatus
        {
            get
            {
                switch (_user.status)
                {
                    case PermissionGroup.Admin:
                        return "Администратор";
                    case PermissionGroup.Moderator:
                        return "Модератор";
                    case PermissionGroup.Operator:
                    default:
                        return "Оператор";
                }
            }
            set 
            {
                OnPropertyChanged(nameof(UserStatus)); 
            }
        }
        public string UserPost
        {
            get
            {
                return _user.post;
            }
            set
            {
                OnPropertyChanged(nameof(UserPost));
            }
        }
        public void RemoveUser()
        {
            _user.RemoveUser();
        }
        public void EditUser()
        {
            UserEditor editor = new(_user);
            editor.ShowDialog();
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(UserStatus));
            OnPropertyChanged(nameof(UserPost));
        }
    }
}
