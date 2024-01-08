using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels
{
    class VM_OpenWorkspace : VM_Base
    {
        private List<string> _workspaces = RegistryData.GetWorkspaces();
        public VM_OpenWorkspace() 
        {
            UpdateUsers();
            UpdateSelectedUser();
        }

        private void UpdateUsers()
        {
            using (User user = new())
            {
                _users.Clear();
                _users = user.GetAllUsers();
            }
            UpdateSelectedUser();
        }

        public void Refresh()
        {
            _workspaces = RegistryData.GetWorkspaces();
            OnPropertyChanged(nameof(Workspaces));
            OnPropertyChanged(nameof(SelectedWorkspace));
            OnPropertyChanged(nameof(Path));
            OnPropertyChanged(nameof(Password));
        }

        public void RemoveSelected()
        {
            RegistryData.RemoveWorkspace(SelectedWorkspace);
            RegistryData.SetSelectedWorkspace("");
            Refresh();
        }

        #region Public Properties
        public List<string> Workspaces
        {
            get { return _workspaces; }
            set
            {
                _workspaces = value;
                OnPropertyChanged(nameof(Workspaces));
            }
        }
        public string SelectedWorkspace
        {
            get
            {
                return RegistryData.GetSelectedWorkspace();
            }
            set
            {
                RegistryData.SetSelectedWorkspace(value);
                ProgramState.SetCommonPath(Path);
                OnPropertyChanged(nameof(SelectedWorkspace));
                OnPropertyChanged(nameof(Path));
                UpdateUsers();
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(Users));
            }
        }
        public string Path
        {
            get
            {
                try
                {
                    return RegistryData.GetWorkspacePath(SelectedWorkspace);
                }
                catch(Exception) { return ""; }
            }
            set
            {
                RegistryData.SetWorkspacePath(SelectedWorkspace, value);
                OnPropertyChanged(nameof(Path));
                OnPropertyChanged(nameof(Users));
            }
        }
        private List<User> _users = new();
        public ObservableCollection<User> Users
        {
            get
            {
                return new ObservableCollection<User>(_users);
            }
            set
            {
                _users = new List<User>(value);
                OnPropertyChanged(nameof(Users));
            }
        }
        public void UpdateSelectedUser()
        {
            try
            {
                string selected = RegistryData.GetWorkspaceSelectedUser(SelectedWorkspace);
                foreach (User user in _users)
                {
                    if (user.id.ToString() == selected)
                    {
                        ProgramState.CurrentUser = user;
                        OnPropertyChanged(nameof(SelectedUser));
                        break;
                    }
                }
            }
            catch(Exception) { }
        }
        public User SelectedUser
        {
            get
            {
                return ProgramState.CurrentUser;
            }
            set
            {
                ProgramState.CurrentUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                try
                {
                    if (value != null)
                    {
                        RegistryData.SetWorkspaceSelectedUser(SelectedWorkspace, value.id.ToString());
                    }
                    
                }
                catch(Exception) { }
            }
        }
        public string Password
        {
            get
            {
                return RegistryData.GetWorkspacePassword(SelectedWorkspace);
            }
            set
            {
                RegistryData.SetWorkspacePassword(SelectedWorkspace, value);
                OnPropertyChanged(nameof(Password));
            }
        }

        #endregion
    }
}
