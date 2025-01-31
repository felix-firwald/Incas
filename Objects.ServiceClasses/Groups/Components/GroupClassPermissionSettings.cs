using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.ServiceClasses.Groups.Components
{
    public class GroupClassPermissionSettings
    {
        /// <summary>
        /// Means can users of this group create and copy objects
        /// </summary>
        [JsonProperty("co")]
        public GroupPermissionType CreateOperations { get; set; }

        /// <summary>
        /// Means can users of this group open objects list
        /// </summary>
        [JsonProperty("vo")]
        public GroupPermissionType ViewOperations { get; set; }

        /// <summary>
        /// Means can users of this group open object card and object editor
        /// </summary>
        [JsonProperty("ro")]
        public GroupPermissionType ReadOperations { get; set; }

        /// <summary>
        /// Means can users of this group update existing objects
        /// </summary>
        [JsonProperty("uo")]
        public GroupPermissionType UpdateOperations { get; set; }

        /// <summary>
        /// Means can users of this group delete objects
        /// </summary>
        [JsonProperty("do")]
        public GroupPermissionType DeleteOperations { get; set; }

        /// <summary>
        /// Means can users of this group manage presets (if it enabled)
        /// <para>Note that this option does not affect the use of presets</para>
        /// </summary>
        [JsonProperty("po")]
        public GroupPermissionType PresetOperations { get; set; }

        /// <summary>
        /// Means can users of this group access confidential fields (if such exist)
        /// </summary>
        [JsonProperty("ca")]
        public GroupPermissionType ConfidentialAccess { get; set; }

        /// <summary>
        /// Only for service classes
        /// </summary>
        /// <returns></returns>
        public bool IsTabVisible()
        {
            if (this.ViewOperations == GroupPermissionType.Allowed)
            {
                return true;
            }
            return false;
        }
        public void SetAll(GroupPermissionType type)
        {
            this.CreateOperations = type;
            this.ViewOperations = type;
            this.ReadOperations = type;
            this.UpdateOperations = type;
            this.DeleteOperations = type;
            this.PresetOperations = type;
            this.ConfidentialAccess = type;
        }
        public void SetOnlyReadViewAllowed()
        {
            this.CreateOperations = GroupPermissionType.Restricted;
            this.ViewOperations = GroupPermissionType.Allowed;
            this.ReadOperations = GroupPermissionType.Allowed;
            this.UpdateOperations = GroupPermissionType.Restricted;
            this.DeleteOperations = GroupPermissionType.Restricted;
            this.PresetOperations = GroupPermissionType.Restricted;
            this.ConfidentialAccess = GroupPermissionType.Allowed;
        }
    }
}
