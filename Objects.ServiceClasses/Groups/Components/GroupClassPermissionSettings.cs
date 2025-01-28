using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.ServiceClasses.Groups.Components
{
    public struct GroupClassPermissionSettings
    {
        /// <summary>
        /// Means can users of this group create and copy objects
        /// </summary>
        public GroupPermissionType CreateOperations { get; set; }
        /// <summary>
        /// Means can users of this group open objects list
        /// </summary>
        public GroupPermissionType ViewOperations { get; set; }
        /// <summary>
        /// Means can users of this group open object card and object editor
        /// </summary>
        public GroupPermissionType ReadOperations { get; set; }
        /// <summary>
        /// Means can users of this group update existing objects
        /// </summary>
        public GroupPermissionType UpdateOperations { get; set; }
        /// <summary>
        /// Means can users of this group delete objects
        /// </summary>
        public GroupPermissionType DeleteOperations { get; set; }
        /// <summary>
        /// Means can users of this group manage presets (if it enabled)
        /// <para>Note that this option does not affect the use of presets</para>
        /// </summary>
        public GroupPermissionType PresetOperations { get; set; }

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
    }
}
