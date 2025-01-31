using Newtonsoft.Json;
using System;

namespace Incas.Objects.Processes.Components
{
    public struct ProcessReview
    {
        [JsonProperty("i")]
        public Guid Id { get; set; }

        [JsonProperty("d")]
        public string Description { get; set; }

        [JsonProperty("ui")]
        public Guid UserId { get; set; }

        [JsonProperty("r")]
        public ReviewResult Result { get; set; }

        [JsonProperty("com")]
        public string Comment { get; set; }
    }
}
