using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.ServiceClasses.Groups.Components
{
    public enum GroupPermissionType
    {
        /// <summary>
        /// if group default is <see cref="GroupPermissionType.Restricted"/> => <see cref="GroupPermissionType.Restricted"/>, otherwise <see cref="GroupPermissionType.Allowed"/>
        /// </summary>
        Default,
        /// <summary>
        /// Means access denied
        /// </summary>
        Restricted,
        /// <summary>
        /// Means passed
        /// </summary>
        Allowed
    }
}
