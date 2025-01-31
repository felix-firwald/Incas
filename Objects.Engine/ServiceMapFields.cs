using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Engine
{
    public class ServiceMapFields
    {
        public List<string> ObjectsMap { get; set; }
        public bool UseEditsMap { get; set; }
        public bool UseAttachmentsMap { get; set; }
        public bool UsePresetsMap { get; set; }
        public bool UseCommentsMap { get; set; }
    }
}
