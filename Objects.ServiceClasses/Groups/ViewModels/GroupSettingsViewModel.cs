using ABI.Windows.AI.MachineLearning;
using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using IncasEngine.Workspace;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Incas.Objects.ServiceClasses.Groups.ViewModels
{
    public class GroupSettingsViewModel : BaseViewModel, IViewModel
    {
        public delegate void ClassArgs(Class cl, GroupClassPermissionSettings settings);
        public event ClassArgs OnOpenMethodsSettingsRequested;
        public GroupData GroupData { get; set; }
        public GroupSettingsViewModel(GroupData data)
        {
            this.GroupData = data;
            this.GetAllClasses();
            this.LoadComponents();
        }
        private void GetAllClasses()
        {
            using (Class cl = new())
            {
                this.customPermissions = new();
                List<ClassItem> classes = cl.GetAllClassItems();
                foreach (ClassItem item in classes)
                {
                    GroupClassPermissionSettings settingsInitState = new();
                    GroupClassPermissionViewModel ps = new(item);
                    ps.OnOpenMethodSettingsRequested += this.ClassPermission_OnOpenMethodSettingsRequested;
                    if (this.GroupData.ClassesPermissions.TryGetValue(item.Id, out settingsInitState)) // if already exists
                    {
                        ps.Settings = settingsInitState;                        
                    }
                    else // if not exists
                    {
                        ps.Settings = new();
                    }
                    this.customPermissions.Add(ps);
                }
            }
            this.OnPropertyChanged(nameof(this.CustomPermissions));
        }

        private void ClassPermission_OnOpenMethodSettingsRequested(Class cl, GroupClassPermissionSettings settings)
        {
            this.OnOpenMethodsSettingsRequested?.Invoke(cl, settings);
        }

        private void LoadComponents()
        {
            ObservableCollection<GroupComponentViewModel> result = new();
            foreach (WorkspaceComponent component in ProgramState.CurrentWorkspace.GetDefinition().Components)
            {
                if (this.GroupData.VisibleComponents.ContainsKey(component.Id))
                {
                    result.Add(new(component, this.GroupData.VisibleComponents[component.Id]));
                }
                else
                {
                    result.Add(new(component, false));
                }
            }
            this.Components = result;
        }
        public bool GeneralSettingsEditing
        {
            get
            {
                return this.GroupData.GeneralSettingsEditing;
            }
            set
            {
                this.GroupData.GeneralSettingsEditing = value;
                this.OnPropertyChanged(nameof(this.GeneralSettingsEditing));
            }
        }
        public bool GlobalParametersEditing
        {
            get
            {
                return this.GroupData.GlobalParametersEditing;
            }
            set
            {
                this.GroupData.GlobalParametersEditing = value;
                this.OnPropertyChanged(nameof(this.GlobalParametersEditing));
            }
        }
        public bool DefaultPermissionType
        {
            get
            {
                return this.FromPermissionType(this.GroupData.DefaultPermissionType);
            }
            set
            {
                this.GroupData.DefaultPermissionType = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.DefaultPermissionType));
            }
        }
        public bool CreatingClasses
        {
            get
            {
                return this.GroupData.CreatingClasses;
            }
            set
            {
                this.GroupData.CreatingClasses = value;
                this.OnPropertyChanged(nameof(this.CreatingClasses));
            }
        }
        public bool UpdatingClasses
        {
            get
            {
                return this.GroupData.UpdatingClasses;
            }
            set
            {
                this.GroupData.UpdatingClasses = value;
                this.OnPropertyChanged(nameof(this.UpdatingClasses));
            }
        }
        public bool RemovingClasses
        {
            get
            {
                return this.GroupData.RemovingClasses;
            }
            set
            {
                this.GroupData.RemovingClasses = value;
                this.OnPropertyChanged(nameof(this.RemovingClasses));
            }
        }
        public bool GroupEditable
        {
            get
            {
                return !this.GroupData.Indestructible;
            }
        }
        #region Groups
        public bool GroupsCreateOperations
        {
            get
            {
                return this.GroupData.GroupClassPermissions.CreateOperations;
            }
            set
            {
                this.GroupData.GroupClassPermissions.CreateOperations = value;
                this.OnPropertyChanged(nameof(this.GroupsCreateOperations));
            }
        }
        public bool GroupsViewOperations
        {
            get
            {
                return this.GroupData.GroupClassPermissions.ViewOperations;
            }
            set
            {
                this.GroupData.GroupClassPermissions.ViewOperations = value;
                this.OnPropertyChanged(nameof(this.GroupsViewOperations));
            }
        }
        public bool GroupsReadOperations
        {
            get
            {
                return this.GroupData.GroupClassPermissions.ReadOperations;
            }
            set
            {
                this.GroupData.GroupClassPermissions.ReadOperations = value;
                this.OnPropertyChanged(nameof(this.GroupsReadOperations));
            }
        }
        public bool GroupsConfidentialAccess
        {
            get
            {
                return this.GroupData.GroupClassPermissions.ConfidentialAccess;
            }
            set
            {
                this.GroupData.GroupClassPermissions.ConfidentialAccess = value;
                this.OnPropertyChanged(nameof(this.GroupsConfidentialAccess));
            }
        }
        public bool GroupsUpdateOperations
        {
            get
            {
                return this.GroupData.GroupClassPermissions.UpdateOperations;
            }
            set
            {
                this.GroupData.GroupClassPermissions.UpdateOperations = value;
                this.OnPropertyChanged(nameof(this.GroupsUpdateOperations));
            }
        }
        public bool GroupsDeleteOperations
        {
            get
            {
                return this.GroupData.GroupClassPermissions.DeleteOperations;
            }
            set
            {
                this.GroupData.GroupClassPermissions.DeleteOperations = value;
                this.OnPropertyChanged(nameof(this.GroupsDeleteOperations));
            }
        }
        #endregion
        #region Users
        public bool UsersCreateOperations
        {
            get
            {
                return this.GroupData.UserClassPermissions.CreateOperations;
            }
            set
            {
                this.GroupData.UserClassPermissions.CreateOperations = value;
                this.OnPropertyChanged(nameof(this.UsersCreateOperations));
            }
        }
        public bool UsersViewOperations
        {
            get
            {
                return this.GroupData.UserClassPermissions.ViewOperations;
            }
            set
            {
                this.GroupData.UserClassPermissions.ViewOperations = value;
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        public bool UsersReadOperations
        {
            get
            {
                return this.GroupData.UserClassPermissions.ReadOperations;
            }
            set
            {
                this.GroupData.UserClassPermissions.ReadOperations = value;
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        public bool UsersConfidentialAccess
        {
            get
            {
                return this.GroupData.UserClassPermissions.ConfidentialAccess;
            }
            set
            {
                this.GroupData.UserClassPermissions.ConfidentialAccess = value;
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        public bool UsersUpdateOperations
        {
            get
            {
                return this.GroupData.UserClassPermissions.UpdateOperations;
            }
            set
            {
                this.GroupData.UserClassPermissions.UpdateOperations = value;
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        public bool UsersDeleteOperations
        {
            get
            {
                return this.GroupData.UserClassPermissions.DeleteOperations;
            }
            set
            {
                this.GroupData.UserClassPermissions.DeleteOperations = value;
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        #endregion
        #region Components
        private ObservableCollection<GroupComponentViewModel> components;
        public ObservableCollection<GroupComponentViewModel> Components
        {
            get
            {
                return this.components;
            }
            set
            {
                this.components = value;
                this.OnPropertyChanged(nameof(this.Components));
            }
        }
        #endregion
        private bool FromPermissionType(GroupPermissionType permissionType)
        {
            switch (permissionType)
            {
                case GroupPermissionType.Default:
                case GroupPermissionType.Restricted:
                default:
                    return false;
                case GroupPermissionType.Allowed:
                    return true;
            }
        }
        private GroupPermissionType ToPermissionType(bool boolean)
        {
            if (boolean)
            {
                return GroupPermissionType.Allowed;
            }
            return GroupPermissionType.Restricted;
        }
        private ObservableCollection<GroupClassPermissionViewModel> customPermissions;
        public ObservableCollection<GroupClassPermissionViewModel> CustomPermissions
        {
            get
            {
                return this.customPermissions;
            }
            set
            {
                this.customPermissions = value;
                this.OnPropertyChanged(nameof(this.CustomPermissions));
            }
        }

        public GroupSettingsViewModel ApplyCustomPermissions()
        {
            this.GroupData.ClassesPermissions = new();
            foreach (GroupClassPermissionViewModel pair in this.CustomPermissions)
            {
                this.GroupData.ClassesPermissions.Add(pair.Item.Id, pair.Settings);
            }
            this.OnPropertyChanged(nameof(this.CustomPermissions));
            return this;
        }
        public void ApplyComponents()
        {
            this.GroupData.VisibleComponents = new();
            foreach (GroupComponentViewModel comp in this.Components)
            {
                this.GroupData.VisibleComponents.Add(comp.Component.Id, comp.IsActivated);
            }
            this.OnPropertyChanged(nameof(this.CustomPermissions));
        }
        //public void SetAllCreateOperations(bool permission)
        //{
        //    foreach (KeyValuePair<ClassItem, GroupClassPermissionSettings> item in this.CustomPermissions)
        //    {
        //        item.Value.CreateOperations = permission;
        //    }
        //    this.OnPropertyChanged(nameof(this.CustomPermissions));
        //}
        //public void SetAllViewOperations(bool permission)
        //{
        //    foreach (KeyValuePair<ClassItem, GroupClassPermissionSettings> item in this.CustomPermissions)
        //    {
        //        item.Value.ViewOperations = permission;
        //    }
        //    this.OnPropertyChanged(nameof(this.CustomPermissions));
        //}
        //public void SetAllReadOperations(bool permission)
        //{
        //    foreach (KeyValuePair<ClassItem, GroupClassPermissionSettings> item in this.CustomPermissions)
        //    {
        //        item.Value.ReadOperations = permission;
        //    }
        //    this.OnPropertyChanged(nameof(this.CustomPermissions));
        //}
        //public void SetAllConfidentialAccess(bool permission)
        //{
        //    foreach (KeyValuePair<ClassItem, GroupClassPermissionSettings> item in this.CustomPermissions)
        //    {
        //        item.Value.ConfidentialAccess = permission;
        //    }
        //    this.OnPropertyChanged(nameof(this.CustomPermissions));
        //}
        //public void SetAllUpdateOperations(bool permission)
        //{
        //    foreach (KeyValuePair<ClassItem, GroupClassPermissionSettings> item in this.CustomPermissions)
        //    {
        //        item.Value.UpdateOperations = permission;
        //    }
        //    this.OnPropertyChanged(nameof(this.CustomPermissions));
        //}
        //public void SetAllDeleteOperations(bool permission)
        //{
        //    foreach (KeyValuePair<ClassItem, GroupClassPermissionSettings> item in this.CustomPermissions)
        //    {
        //        item.Value.DeleteOperations = permission;
        //    }
        //    this.OnPropertyChanged(nameof(this.CustomPermissions));
        //}
    }
}
