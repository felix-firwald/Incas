using System.Collections.Generic;

namespace Incas.Objects.Components
{
    public struct TemplateData
    {
        public string Name { get; set; }
        public string File { get; set; }
        public List<TemplateConditionalAction> Actions { get; set; }
    }
}
