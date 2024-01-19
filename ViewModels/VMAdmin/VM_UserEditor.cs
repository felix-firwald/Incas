using Common;
using Incubator_2.Common;
using Incubator_2.Models;
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
        private UserParameters _userParameters;
        private List<Sector> _sectors;
        private Sector _selectedSector;
        public VM_UserEditor(User user)
        {
            _user = user;
            if (user.id > 0 )
            {
                _userParameters = user.GetParametersContext();
                using (Sector s = new())
                {
                    Sectors = s.GetSectors();
                    s.slug = _user.sector;
                    _selectedSector = FindSector(s.slug);
                    OnPropertyChanged(nameof(SelectedSector));
                }
            }
            else
            {
                using (Sector s = new())
                {
                    Sectors = s.GetSectors();
                }
                _userParameters = new();
            }
        }
        private Sector FindSector(string slug)
        {
            foreach (Sector s in Sectors)
            {
                if (s.slug == slug)
                {
                    return s;
                }
            }
            return new();
        }
        public List<Sector> Sectors
        {
            get
            {
                return _sectors;
            }
            set
            {
                _sectors = value;
                OnPropertyChanged(nameof(Sectors));
            }
        }

        public Sector SelectedSector
        {
            get
            {
                return _selectedSector;
            }
            set
            {
                _selectedSector = value;
                OnPropertyChanged(nameof(SelectedSector));
            }
        }

        public string PermissionStatus
        {
            get
            {
                if (_user.id == 1)
                {
                    return ParseFromEnum(PermissionGroup.Admin);
                }
                return ParseFromEnum(_userParameters.permission_group);
            }
            set
            {
                _userParameters.permission_group = ParseToEnum(value);
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
        public bool StatusEditable
        {
            get
            {
                if (_user.id == 1 || _user.username == "admin") // если это первый админ
                {
                    return false;
                }
                return true;
            }
        }
        public bool UsernameEditable
        {
            get
            {
                return _user.id == 0;
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
        public string SecondName
        {
            get
            {
                return _user.secondName;
            }
            set
            {
                _user.secondName = value;
                OnPropertyChanged(nameof(SecondName));
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
                return _userParameters.startup_password;
            }
            set
            {
                _userParameters.startup_password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public bool CanViewTasks
        {
            get
            {
                return _userParameters.tasks_visibility;
            }
            set
            {
                _userParameters.tasks_visibility = value;
                OnPropertyChanged(nameof(CanViewTasks));
            }
        }
        public bool CanViewCommunication
        {
            get
            {
                return _userParameters.communication_visibility;
            }
            set
            {
                _userParameters.communication_visibility = value;
                OnPropertyChanged(nameof(CanViewCommunication));
            }
        }
        public bool CanViewDatabase
        {
            get
            {
                return _userParameters.database_visibility;
            }
            set
            {
                _userParameters.database_visibility = value;
                OnPropertyChanged(nameof(CanViewDatabase));
            }
        }
        public bool CanCreateTemplates
        {
            get
            {
                return _userParameters.create_templates;
            }
            set
            {
                _userParameters.create_templates = value;
                OnPropertyChanged(nameof(CanCreateTemplates));
            }
        }
        public bool CanModifyTemplates
        {
            get
            {
                return _userParameters.modify_templates;
            }
            set
            {
                _userParameters.modify_templates = value;
                OnPropertyChanged(nameof(CanModifyTemplates));
            }
        }
        public bool WritesLog
        {
            get
            {
                return _userParameters.write_log;
            }
            set
            {
                _userParameters.write_log = value;
                OnPropertyChanged(nameof(WritesLog));
            }
        }
        public bool WritesListenLog
        {
            get
            {
                return _userParameters.write_listen_log;
            }
            set
            {
                _userParameters.write_listen_log = value;
                OnPropertyChanged(nameof(WritesListenLog));
            }
        }


        public bool Save()
        {
            if (!string.IsNullOrWhiteSpace(_user.username) && !string.IsNullOrWhiteSpace(_userParameters.startup_password) && !string.IsNullOrWhiteSpace(_user.surname) && _selectedSector != null)
            {
                ProgramState.ShowWaitCursor();
                _user.fullname = $"{_user.surname} {_user.secondName}";
                _user.sector = _selectedSector.slug;
                _user.SaveUser();
                UserContextor.SaveContext(_userParameters, _user);
                ProgramState.ShowWaitCursor(false);
                return true;
            }
            return false;
        }
    }
}
