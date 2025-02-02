using Incas.Core.Classes;
using Incas.Objects.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Incas.Objects.ServiceClasses.Groups.Components
{
    public class GroupData : IObjectData
    {
        [JsonProperty("g_dpt")]
        public GroupPermissionType DefaultPermissionType { get; set; }

        /// <summary>
        /// Can users of this group edit general settings of workspace
        /// </summary>
        [JsonProperty("g_gs")]
        public bool GeneralSettingsEditing { get; set; }

        /// <summary>
        /// Can users of this group create classes
        /// </summary>
        [JsonProperty("g_cc")]
        public bool CreatingClasses { get; set; }

        /// <summary>
        /// Can users of this group update classes
        /// </summary>
        [JsonProperty("g_uc")]
        public bool UpdatingClasses { get; set; }

        /// <summary>
        /// Can users of this group remove classes
        /// </summary>
        [JsonProperty("g_rc")]
        public bool RemovingClasses { get; set; }

        /// <summary>
        /// Means user (including super admin) can`t remove or edit this group
        /// <para>For the first group INCAS sets it to True, otherwise False</para>
        /// </summary>
        [JsonProperty("g_i")]
        public bool Indestructible { get; set; }

        /// <summary>
        /// Can users of this group access with user service class
        /// </summary>
        [JsonProperty("sc_u")]
        public GroupClassPermissionSettings UserClassPermissions { get; set; }

        /// <summary>
        /// Can users of this group access with group service class
        /// </summary>
        [JsonProperty("sc_g")]
        public GroupClassPermissionSettings GroupClassPermissions { get; set; }

        /// <summary>
        /// Custom permissions for classes
        /// </summary>
        [JsonProperty("cp")]
        public Dictionary<Guid, GroupClassPermissionSettings> ClassesPermissions { get; set; }

        public GroupData()
        {
            this.UserClassPermissions = new();
            this.GroupClassPermissions = new();
            this.ClassesPermissions = new();
        }

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
            if (this.ClassesPermissions is null)
            {
                this.ClassesPermissions = new();
            }
            try
            {
                result = this.ClassesPermissions[classId];
            }
            catch
            {
                WorkspaceDefinition wps = ProgramState.CurrentWorkspace.GetDefinition();
                if (wps.ServiceGroups.Id == classId)
                {
                    result = this.GroupClassPermissions;
                }
                else if (wps.ServiceUsers.Id == classId)
                {
                    result = this.UserClassPermissions;
                }
                else
                {
                    bool defaultBoolean = this.DefaultPermissionType == GroupPermissionType.Allowed;
                    result.CreateOperations = defaultBoolean;
                    result.ReadOperations = defaultBoolean;
                    result.ViewOperations = defaultBoolean;
                    result.UpdateOperations = defaultBoolean;
                    result.DeleteOperations = defaultBoolean;
                    result.PresetOperations = defaultBoolean;
                }               
            }
            return result;
        }       
    }
}
