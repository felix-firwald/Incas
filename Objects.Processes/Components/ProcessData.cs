using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Incas.Objects.Processes.Components
{
    public class ProcessData
    {
        public string Description { get; set; }
        public List<ProcessDocumentLink> Documents { get; set; }
        public List<ProcessTask> ClassTasks { get; set; }
        public List<ProcessTask> CustomTasks { get; set; }
        public List<ProcessReview> Reviews { get; set; }
    }
}
