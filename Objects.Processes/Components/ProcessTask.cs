using Newtonsoft.Json;
using System;

namespace Incas.Objects.Processes.Components
{
    public struct ProcessTask
    {
        [JsonProperty("i")]
        public Guid Id { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("ui")]
        public Guid UserId { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }
    }
}
