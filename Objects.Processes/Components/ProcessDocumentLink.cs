using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Processes.Components
{
    public struct ProcessDocumentLink
    {
        [JsonProperty("i")]
        public Guid Id { get; set; }

        [JsonProperty("ci")]
        public Guid ClassId { get; set; }

        [JsonProperty("di")]
        public Guid DocumentId { get; set; }

        [JsonProperty("fin")]
        public bool Finished { get; set; }
    }
}
