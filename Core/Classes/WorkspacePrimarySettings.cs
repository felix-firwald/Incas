using Incas.Objects.ServiceClasses.Models;

namespace Incas.Core.Classes
{
    public struct WorkspacePrimarySettings
    {
        public string Name { get; set; }
        public ServiceClass ServiceGroups { get; set; }
        public ServiceClass ServiceUsers { get; set; }
        public bool IsLocked { get; set; }
    }
}
