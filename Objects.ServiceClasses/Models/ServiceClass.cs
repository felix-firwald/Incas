using Incas.Objects.Components;
using Incas.Objects.Engine;
using Incas.Objects.Models;
using Incas.Objects.ServiceClasses.Components;
using Newtonsoft.Json;
using System;

namespace Incas.Objects.ServiceClasses.Models
{
    public class ServiceClass : IClass
    {
        [JsonProperty("i")]
        public Guid Id { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("t")]
        public ClassType Type { get; set; }

        [JsonProperty("d")]
        public ClassData Data { get; set; }

        public ClassData GetClassData()
        {
            return this.Data;
        }
    }
}
