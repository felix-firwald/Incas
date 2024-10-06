using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    public class TemplateConditionalAction
    {
        public TemplateCondition Condition { get; set; }
        public TemplateAction Action { get; set; }
    }
}
