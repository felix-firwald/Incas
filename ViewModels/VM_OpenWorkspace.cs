using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        private void UpdateUsers()
        {
            using (User user = new())
            {
                _users.Clear();
                _users = user.GetAllUsers();
            }
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
                return RegistryData.GetWorkspacePath(SelectedWorkspace);
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
