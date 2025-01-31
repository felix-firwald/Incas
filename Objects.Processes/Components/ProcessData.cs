using Incas.Objects.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Incas.Objects.Processes.Components
{
    public class ProcessData : IObjectData
    {
        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("docs")]
        public List<ProcessDocumentLink> Documents { get; set; }

        [JsonProperty("ct")]
        public List<ProcessTask> ClassTasks { get; set; }

        [JsonProperty("ot")]
        public List<ProcessTask> CustomTasks { get; set; }

        [JsonProperty("rev")]
        public List<ProcessReview> Reviews { get; set; }
    }
}
