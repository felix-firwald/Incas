using Incas.Objects.ServiceClasses.Models;
using Newtonsoft.Json;
using System;

namespace Incas.Core.Classes
{
    public class WorkspacePrimarySettings
    {
        [JsonProperty("i")]
        public Guid Id { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("sc_g")]
        public ServiceClass ServiceGroups { get; set; }

        [JsonProperty("sc_u")]
        public ServiceClass ServiceUsers { get; set; }

        [JsonProperty("l")]
        public bool IsLocked { get; set; }

        [JsonProperty("scrp")]
        public bool SignificantChangesRequirePassword { get; set; }
    }
}
