using Incas.Core.Classes;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Users.Components;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Incas.Core.ViewModels
{
    internal class OpenWorkspaceViewModel : BaseViewModel
    {
        private List<string> _workspaces = RegistryData.GetWorkspaces();
        public OpenWorkspaceViewModel()
        {
            //this.UpdateUsers();
            //this.UpdateSelectedUser();
            this.SelectedWorkspace = RegistryData.GetSelectedWorkspace();
        }

        private void UpdateUsers()
        {
            if (RegistryData.IsWorkspaceExists(this.SelectedWorkspace))
            {                
                this._users.Clear();
                ServiceClass userClass = ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers;
                this._users = User.GetItems(Processor.GetSimpleObjectsList(userClass));
                this.UpdateSelectedUser();
            }
        }

        public void Refresh()
        {
            this._workspaces = RegistryData.GetWorkspaces();
            this.OnPropertyChanged(nameof(this.Workspaces));
            this.OnPropertyChanged(nameof(this.SelectedWorkspace));
            this.OnPropertyChanged(nameof(this.Path));
            this.UpdateUsers();
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
            WorkspacePaths.SetCommonPath(this.Path);
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
        private List<UserItem> _users = [];
        public ObservableCollection<UserItem> Users
        {
            get => new(this._users);
            set
            {
                this._users = new List<UserItem>(value);
                this.OnPropertyChanged(nameof(this.Users));
            }
        }
        public void UpdateSelectedUser()
        {
            try
            {
                string selected = RegistryData.GetWorkspaceSelectedUser(this.SelectedWorkspace);
                foreach (UserItem user in this._users)
                {
                    if (user.Id.ToString() == selected)
                    {

                        this.SelectedUser = user;
                        this.OnPropertyChanged(nameof(this.SelectedUser));
                        break;
                    }
                }
            }
            catch (Exception) { }
        }
        public UserItem SelectedUser
        {
            get
            {
                if (ProgramState.CurrentWorkspace.CurrentUser is null)
                {
                    return new();
                }
                return ProgramState.CurrentWorkspace.CurrentUser.AsItem();
            }
            set
            {
                ProgramState.CurrentWorkspace.CurrentUser = (User)Processor.GetObject(ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers, value.Id);
                this.OnPropertyChanged(nameof(this.SelectedUser));
                try
                {
                    RegistryData.SetWorkspaceSelectedUser(this.SelectedWorkspace, value.Id.ToString());
                }
                catch (Exception) { }
            }
        }
        //private string pwd = "";
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
