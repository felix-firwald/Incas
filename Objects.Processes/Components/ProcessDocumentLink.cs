using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Processes.Components
{
    public struct ProcessDocumentLink
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Guid DocumentId { get; set; }
        public bool Finished { get; set; }
    }
}
