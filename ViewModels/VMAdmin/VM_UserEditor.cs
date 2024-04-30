using Common;
using Incubator_2.Common;
using Incubator_2.Models;
using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels.VMAdmin
{
    internal class VM_UserEditor : VM_Base
    {
        private User _user;
        private UserParameters _userParameters;
        private List<Sector> _sectors;
        private Sector _selectedSector;
        public VM_UserEditor(User user)
        {
            this._user = user;
            if (user.id > 0)
            {
                this._userParameters = user.GetParametersContext();
                using Sector s = new();
                this.Sectors = s.GetSectors();
                s.slug = this._user.sector;
                this._selectedSector = this.FindSector(s.slug);
                this.OnPropertyChanged(nameof(this.SelectedSector));
            }
            else
            {
                using (Sector s = new())
                {
                    this.Sectors = s.GetSectors();
                }
                this._userParameters = new();
            }
        }
        private Sector FindSector(string slug)
        {
            foreach (Sector s in this.Sectors)
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
                return this._sectors;
            }
            set
            {
                this._sectors = value;
                this.OnPropertyChanged(nameof(this.Sectors));
            }
        }

        public Sector SelectedSector
        {
            get
            {
                return this._selectedSector;
            }
            set
            {
                this._selectedSector = value;
                this.OnPropertyChanged(nameof(this.SelectedSector));
            }
        }

        public string PermissionStatus
        {
            get
            {
                if (this._user.id == 1)
                {
                    return this.ParseFromEnum(PermissionGroup.Admin);
                }
                return this.ParseFromEnum(this._userParameters.permission_group);
            }
            set
            {
                this._userParameters.permission_group = this.ParseToEnum(value);
                this.OnPropertyChanged(nameof(this.PermissionStatus));
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
                case PermissionGroup.Editor:
                    return "2";
                case PermissionGroup.Operator:
                default:
                    return "3";
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
                    return PermissionGroup.Editor;
                case "3":
                default:
                    return PermissionGroup.Operator;
            }
        }
        public bool StatusEditable
        {
            get
            {
                if (this._user.id == 1 || this._user.username == "admin") // если это первый админ
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
                return this._user.id == 0;
            }
        }
        public string Username
        {
            get
            {
                return this._user.username;
            }
            set
            {
                this._user.username = value;
                this.OnPropertyChanged(nameof(this.Username));
            }
        }
        public string Surname
        {
            get
            {
                return this._user.surname;
            }
            set
            {
                this._user.surname = value;
                this.OnPropertyChanged(nameof(this.Surname));
            }
        }
        public string SecondName
        {
            get
            {
                return this._user.secondName;
            }
            set
            {
                this._user.secondName = value;
                this.OnPropertyChanged(nameof(this.SecondName));
            }
        }

        public string Post
        {
            get
            {
                return this._user.post;
            }
            set
            {
                this._user.post = value;
                this.OnPropertyChanged(nameof(this.Post));
            }
        }
        public string Password
        {
            get
            {
                return this._userParameters.startup_password;
            }
            set
            {
                this._userParameters.startup_password = value;
                this.OnPropertyChanged(nameof(this.Password));
            }
        }

        public bool CanViewTasks
        {
            get
            {
                return this._userParameters.tasks_visibility;
            }
            set
            {
                this._userParameters.tasks_visibility = value;
                this.OnPropertyChanged(nameof(this.CanViewTasks));
            }
        }
        public bool CanViewCommunication
        {
            get
            {
                return this._userParameters.communication_visibility;
            }
            set
            {
                this._userParameters.communication_visibility = value;
                this.OnPropertyChanged(nameof(this.CanViewCommunication));
            }
        }
        public bool CanViewDatabase
        {
            get
            {
                return this._userParameters.database_visibility;
            }
            set
            {
                this._userParameters.database_visibility = value;
                this.OnPropertyChanged(nameof(this.CanViewDatabase));
            }
        }
        public bool CanCreateTemplates
        {
            get
            {
                return this._userParameters.create_templates;
            }
            set
            {
                this._userParameters.create_templates = value;
                this.OnPropertyChanged(nameof(this.CanCreateTemplates));
            }
        }
        public bool CanModifyTemplates
        {
            get
            {
                return this._userParameters.modify_templates;
            }
            set
            {
                this._userParameters.modify_templates = value;
                this.OnPropertyChanged(nameof(this.CanModifyTemplates));
            }
        }
        public bool WritesLog
        {
            get
            {
                return this._userParameters.write_log;
            }
            set
            {
                this._userParameters.write_log = value;
                this.OnPropertyChanged(nameof(this.WritesLog));
            }
        }
        public bool WritesListenLog
        {
            get
            {
                return this._userParameters.write_listen_log;
            }
            set
            {
                this._userParameters.write_listen_log = value;
                this.OnPropertyChanged(nameof(this.WritesListenLog));
            }
        }


        public bool Save()
        {
            if (!string.IsNullOrWhiteSpace(this._user.username) && !string.IsNullOrWhiteSpace(this._userParameters.startup_password) && !string.IsNullOrWhiteSpace(this._user.surname) && this._selectedSector != null)
            {
                ProgramState.ShowWaitCursor();
                this._user.fullname = $"{this._user.surname} {this._user.secondName}";
                this._user.sector = this._selectedSector.slug;
                this._user.GenerateSign();
                this._user.SaveParametersContext(this._userParameters);
                this._user.SaveUser();
                ProgramState.ShowWaitCursor(false);
                return true;
            }
            return false;
        }
    }
}
