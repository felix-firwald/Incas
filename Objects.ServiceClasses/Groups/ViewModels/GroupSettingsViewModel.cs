using Incas.Core.ViewModels;
using Incas.Objects.Engine;
using Incas.Objects.ServiceClasses.Groups.Components;

namespace Incas.Objects.ServiceClasses.Groups.ViewModels
{
    public class GroupSettingsViewModel : BaseViewModel, IViewModel
    {
        public GroupData GroupData { get; set; }
        public GroupSettingsViewModel(GroupData data)
        {
            this.GroupData = data;
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
                return this.FromPermissionType(this.GroupData.GroupClassPermissions.CreateOperations);
            }
            set
            {
                this.GroupData.GroupClassPermissions.CreateOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.GroupsCreateOperations));
            }
        }
        public bool GroupsViewOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.GroupClassPermissions.ViewOperations);
            }
            set
            {
                this.GroupData.GroupClassPermissions.ViewOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.GroupsViewOperations));
            }
        }
        public bool GroupsReadOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.GroupClassPermissions.ReadOperations);
            }
            set
            {
                this.GroupData.GroupClassPermissions.ReadOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.GroupsReadOperations));
            }
        }
        public bool GroupsConfidentialAccess
        {
            get
            {
                return this.FromPermissionType(this.GroupData.GroupClassPermissions.ConfidentialAccess);
            }
            set
            {
                this.GroupData.GroupClassPermissions.ConfidentialAccess = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.GroupsConfidentialAccess));
            }
        }
        public bool GroupsUpdateOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.GroupClassPermissions.UpdateOperations);
            }
            set
            {
                this.GroupData.GroupClassPermissions.UpdateOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.GroupsUpdateOperations));
            }
        }
        public bool GroupsDeleteOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.GroupClassPermissions.DeleteOperations);
            }
            set
            {
                this.GroupData.GroupClassPermissions.DeleteOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.GroupsDeleteOperations));
            }
        }
        #endregion
        #region Users
        public bool UsersCreateOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.UserClassPermissions.CreateOperations);
            }
            set
            {
                this.GroupData.UserClassPermissions.CreateOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.UsersCreateOperations));
            }
        }
        public bool UsersViewOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.UserClassPermissions.ViewOperations);
            }
            set
            {
                this.GroupData.UserClassPermissions.ViewOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        public bool UsersReadOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.UserClassPermissions.ReadOperations);
            }
            set
            {
                this.GroupData.UserClassPermissions.ReadOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        public bool UsersConfidentialAccess
        {
            get
            {
                return this.FromPermissionType(this.GroupData.UserClassPermissions.ConfidentialAccess);
            }
            set
            {
                this.GroupData.UserClassPermissions.ConfidentialAccess = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        public bool UsersUpdateOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.UserClassPermissions.UpdateOperations);
            }
            set
            {
                this.GroupData.UserClassPermissions.UpdateOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
            }
        }
        public bool UsersDeleteOperations
        {
            get
            {
                return this.FromPermissionType(this.GroupData.UserClassPermissions.DeleteOperations);
            }
            set
            {
                this.GroupData.UserClassPermissions.DeleteOperations = this.ToPermissionType(value);
                this.OnPropertyChanged(nameof(this.UsersDeleteOperations));
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
    }
}
