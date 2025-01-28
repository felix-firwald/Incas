using System;
using System.Collections.Generic;

namespace Incas.Objects.ServiceClasses.Groups.Components
{
    public class GroupData
    {
        public GroupPermissionType DefaultPermissionType { get; set; }
        /// <summary>
        /// Can users of this group edit general settings of workspace
        /// </summary>
        public bool GeneralSettingsEditing { get; set; }
        /// <summary>
        /// Can users of this group create classes
        /// </summary>
        public bool CreatingClasses { get; set; }
        /// <summary>
        /// Can users of this group update classes
        /// </summary>
        public bool UpdatingClasses { get; set; }
        /// <summary>
        /// Can users of this group remove classes
        /// </summary>
        public bool RemovingClasses { get; set; }
        /// <summary>
        /// Can users of this group remove classes
        /// </summary>
        public GroupClassPermissionSettings UserClassPermissions { get; set; }
        public GroupClassPermissionSettings GroupClassPermissions { get; set; }
        public Dictionary<Guid, GroupClassPermissionSettings> ClassesPermissions { get; set; }

        public bool CanGroupViewingWorkspaceTab()
        {
            if (this.GeneralSettingsEditing || this.CreatingClasses || this.UpdatingClasses || this.RemovingClasses)
            {
                return true;
            }
            return false;
        }
        public GroupClassPermissionSettings GetClassPermissions(Guid classId)
        {
            GroupClassPermissionSettings result = new();
            if (this.ClassesPermissions.TryGetValue(classId, out GroupClassPermissionSettings settings) == false)
            {
                result.CreateOperations = this.DefaultPermissionType;
                result.ReadOperations = this.DefaultPermissionType;
                result.ViewOperations = this.DefaultPermissionType;
                result.UpdateOperations = this.DefaultPermissionType;
                result.DeleteOperations = this.DefaultPermissionType;
                result.PresetOperations = this.DefaultPermissionType;
            }
            return result;
        }
    }
}
