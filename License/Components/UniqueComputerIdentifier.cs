using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.License.Components
{
    internal struct UniqueComputerIdentifier
    {
        [JsonProperty("mn")]
        public string MachineName { get; set; }
        [JsonProperty("udn")]
        public string UserDomainName { get; set; }
        [JsonProperty("umi")]
        public string UniqueMachineId { get; set; }
    }
}
