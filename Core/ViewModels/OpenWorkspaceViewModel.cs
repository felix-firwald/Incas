using Incas.Core.Classes;
using IncasEngine.Core.Registry;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Users.Components;
using IncasEngine.Workspace;
using IncasEngine.Workspace.WorkspaceConnectionPaths;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Incas.Core.ViewModels
{
    internal class OpenWorkspaceViewModel : BaseViewModel
    {
        private List<WorkspaceConnection> _workspaces = WorkspacePaths.GetWorkspaces();
        public OpenWorkspaceViewModel()
        {
            this.SelectedWorkspace = WorkspacePaths.GetSelectedConnection();
        }

        private void UpdateUsers()
        {
            if (this.SelectedWorkspace is not null)
            {                
                this._users.Clear();
                ServiceClass userClass = ProgramState.CurrentWorkspace.GetDefinition().ServiceUsers;
                this._users = User.GetItems(Processor.GetSimpleObjectsList(userClass));
                this.UpdateSelectedUser();
            }
        }

        public void Refresh()
        {
            this.OnPropertyChanged(nameof(this.Workspaces));
            this.OnPropertyChanged(nameof(this.SelectedWorkspace));
            this.UpdateUsers();
        }

        public void RemoveSelected()
        {
            WorkspacePaths.RemoveWorkspaceConnection(this.SelectedWorkspace);
            this.Refresh();
        }

        #region Public Properties
        public List<WorkspaceConnection> Workspaces
        {
            get => WorkspacePaths.GetWorkspaces();
        }
        private WorkspaceConnection selectedWorkspace = null;
        public WorkspaceConnection SelectedWorkspace
        {
            get => selectedWorkspace;
            set
            {
                this.selectedWorkspace = value;
                WorkspacePaths.SetSelectedConnection(value);
                if (this.SetPath())
                {
                    this.OnPropertyChanged(nameof(this.SelectedWorkspace));
                    //this.OnPropertyChanged(nameof(this.Path));
                    this.UpdateUsers();
                    this.OnPropertyChanged(nameof(this.Users));
                }
            }
        }
        public bool SetPath()
        {
            if (this.SelectedWorkspace is not null)
            {
                WorkspacePaths.SetCommonPath(this.SelectedWorkspace.Path);
                return true;
            }
            return false;
        }
        //public string Path
        //{
        //    get
        //    {
        //        try
        //        {
        //            return this.SelectedWorkspace);
        //        }
        //        catch (Exception)
        //        {
        //            return "";
        //        }
        //    }
        //    set
        //    {
        //        WorkspacePaths.SetWorkspacePath(this.SelectedWorkspace, value);
        //        if (WorkspacePaths.IsWorkspaceExists(this.SelectedWorkspace))
        //        {
        //            this.OnPropertyChanged(nameof(this.Path));
        //            this.OnPropertyChanged(nameof(this.Users));
        //        }
        //    }
        //}
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
                Guid selected = this.SelectedWorkspace.DefaultSelectedUser;
                foreach (UserItem user in this._users)
                {
                    if (user.Id == selected)
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
                if (ProgramState.CurrentWorkspace?.CurrentUser is null)
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
                    this.SelectedWorkspace.DefaultSelectedUser = value.Id;
                    //WorkspacePaths.SaveConfig();
                }
                catch (Exception) { }
            }
        }
        #endregion
    }
}
