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
        public bool CreateOperations { get; set; }

        /// <summary>
        /// Means can users of this group open objects list
        /// </summary>
        [JsonProperty("vo")]
        public bool ViewOperations { get; set; }

        /// <summary>
        /// Means can users of this group open object card and object editor
        /// </summary>
        [JsonProperty("ro")]
        public bool ReadOperations { get; set; }

        /// <summary>
        /// Means can users of this group update existing objects
        /// </summary>
        [JsonProperty("uo")]
        public bool UpdateOperations { get; set; }

        /// <summary>
        /// Means can users of this group delete objects
        /// </summary>
        [JsonProperty("do")]
        public bool DeleteOperations { get; set; }

        /// <summary>
        /// Means can users of this group manage presets (if it enabled)
        /// <para>Note that this option does not affect the use of presets</para>
        /// </summary>
        [JsonProperty("po")]
        public bool PresetOperations { get; set; }

        /// <summary>
        /// Means can users of this group access confidential fields (if such exist)
        /// </summary>
        [JsonProperty("ca")]
        public bool ConfidentialAccess { get; set; }

        /// <summary>
        /// Only for service classes
        /// </summary>
        /// <returns></returns>
        public bool IsTabVisible()
        {
            return this.ViewOperations;
        }
        public void SetAll(GroupPermissionType type)
        {
            bool result = false;
            switch (type)
            {
                case GroupPermissionType.Default:
                case GroupPermissionType.Restricted:
                    result = false;
                    break;
                case GroupPermissionType.Allowed:
                    result = true;
                    break;
            }
            this.CreateOperations = result;
            this.ViewOperations = result;
            this.ReadOperations = result;
            this.UpdateOperations = result;
            this.DeleteOperations = result;
            this.PresetOperations = result;
            this.ConfidentialAccess = result;
        }
        public void SetOnlyReadViewAllowed()
        {
            this.CreateOperations = false;
            this.ViewOperations = true;
            this.ReadOperations = true;
            this.UpdateOperations = false;
            this.DeleteOperations = false;
            this.PresetOperations = false;
            this.ConfidentialAccess = true;
        }
    }
}
