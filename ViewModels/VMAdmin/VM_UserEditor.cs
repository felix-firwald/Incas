using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VMAdmin
{
    class VM_UserEditor : VM_Base
    {
        private User _user;
        public VM_UserEditor(User user)
        {
            _user = user;
        }
        public string PermissionStatus
        {
            get
            {
                return ParseFromEnum(_user.status);
            }
            set
            {
                _user.status = ParseToEnum(value);
                OnPropertyChanged(nameof(PermissionStatus));
            }
        }
        private string ParseFromEnum(PermissionGroup permission)
        {
            switch (permission)
            {
                case PermissionGroup.Admin:
                    return "0";
                case PermissionGroup.Moderator:
                    return "1";
                case PermissionGroup.Operator:
                default:
                    return "2";
            }
        }
        private PermissionGroup ParseToEnum(string index)
        {
            switch (index)
            {
                case "0":
                    return PermissionGroup.Admin;
                case "1":
                    return PermissionGroup.Moderator;
                case "2":
                default:
                    return PermissionGroup.Operator;
            }
        }
        public string Username
        {
            get
            {
                return _user.username;
            }
            set
            {
                _user.username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Surname
        {
            get
            {
                return _user.surname;
            }
            set
            {
                _user.surname = value;
                OnPropertyChanged(nameof(Surname));
            }
        }
        public string Fullname
        {
            get
            {
                return _user.fullname;
            }
            set
            {
                _user.fullname = value;
                OnPropertyChanged(nameof(Fullname));
            }
        }
        public string Post
        {
            get
            {
                return _user.post;
            }
            set
            {
                _user.post = value;
                OnPropertyChanged(nameof(Post));
            }
        }
        public string Password
        {
            get
            {
                return _user.password;
            }
            set
            {
                _user.password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public void Save()
        {
            _user.SaveUser();
        }
    }
}
