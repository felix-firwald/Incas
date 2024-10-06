using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    public struct TemplateData
    {
        public string Name { get; set; }
        public string File { get; set; }
        public List<TemplateConditionalAction> Actions { get; set; }
    }
}
