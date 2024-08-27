using Incas.Core.Classes;
using Incas.Core.Exceptions;
using Incas.Users.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Incas.Core.ViewModels
{
    internal class OpenWorkspaceViewModel : BaseViewModel
    {
        private List<string> _workspaces = RegistryData.GetWorkspaces();
        public OpenWorkspaceViewModel()
        {
            this.UpdateUsers();
            this.UpdateSelectedUser();
        }

        private void UpdateUsers()
        {
            if (RegistryData.IsWorkspaceExists(this.SelectedWorkspace))
            {
                using (User user = new())
                {
                    this._users.Clear();
                    this._users = user.GetAllUsers();
                    //if (this._users.Count == 0)
                    //{
                    //    DialogsManager.ShowErrorDialog("В рабочем пространстве не найдено ни одного пользователя, что означает его неисправность.", "Ошибка загрузки");
                    //}
                }
                this.UpdateSelectedUser();
            }
        }

        public void Refresh()
        {
            this._workspaces = RegistryData.GetWorkspaces();
            this.OnPropertyChanged(nameof(this.Workspaces));
            this.OnPropertyChanged(nameof(this.SelectedWorkspace));
            this.OnPropertyChanged(nameof(this.Path));
            this.OnPropertyChanged(nameof(this.Password));
        }

        public void RemoveSelected()
        {
            RegistryData.RemoveWorkspace(this.SelectedWorkspace);
            RegistryData.SetSelectedWorkspace("");
            this.Refresh();
        }

        #region Public Properties
        public List<string> Workspaces
        {
            get => this._workspaces;
            set
            {
                this._workspaces = value;
                this.OnPropertyChanged(nameof(this.Workspaces));
            }
        }
        public string SelectedWorkspace
        {
            get => RegistryData.GetSelectedWorkspace();
            set
            {
                RegistryData.SetSelectedWorkspace(value);
                if (this.SetPath())
                {
                    this.OnPropertyChanged(nameof(this.SelectedWorkspace));
                    this.OnPropertyChanged(nameof(this.Path));
                    this.UpdateUsers();
                    this.OnPropertyChanged(nameof(this.Password));
                    this.OnPropertyChanged(nameof(this.Users));
                }                
            }
        }
        public bool SetPath()
        {
            ProgramState.SetCommonPath(this.Path);
            return true;
        }
        public string Path
        {
            get
            {
                try
                {
                    return RegistryData.GetWorkspacePath(this.SelectedWorkspace);
                }
                catch (Exception) { return ""; }
            }
            set
            {
                RegistryData.SetWorkspacePath(this.SelectedWorkspace, value);
                if (RegistryData.IsWorkspaceExists(this.SelectedWorkspace))
                {
                    this.OnPropertyChanged(nameof(this.Path));
                    this.OnPropertyChanged(nameof(this.Users));
                }
            }
        }
        private List<User> _users = [];
        public ObservableCollection<User> Users
        {
            get => new ObservableCollection<User>(this._users);
            set
            {
                this._users = new List<User>(value);
                this.OnPropertyChanged(nameof(this.Users));
            }
        }
        public void UpdateSelectedUser()
        {
            try
            {
                string selected = RegistryData.GetWorkspaceSelectedUser(this.SelectedWorkspace);
                foreach (User user in this._users)
                {
                    if (user.id.ToString() == selected)
                    {
                        ProgramState.CurrentUser = user;
                        this.OnPropertyChanged(nameof(this.SelectedUser));
                        break;
                    }
                }
            }
            catch (Exception) { }
        }
        public User SelectedUser
        {
            get => ProgramState.CurrentUser;
            set
            {
                ProgramState.CurrentUser = value;
                this.OnPropertyChanged(nameof(this.SelectedUser));
                try
                {
                    if (value != null)
                    {
                        RegistryData.SetWorkspaceSelectedUser(this.SelectedWorkspace, value.id.ToString());
                    }
                }
                catch (Exception) { }
            }
        }
        public string Password
        {
            get => RegistryData.GetWorkspacePassword(this.SelectedWorkspace);
            set
            {
                RegistryData.SetWorkspacePassword(this.SelectedWorkspace, value);
                this.OnPropertyChanged(nameof(this.Password));
            }
        }

        #endregion
    }
}
