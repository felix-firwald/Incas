using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    public class TemplateCondition
    {
        public Guid Field { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public string TargetValue { get; set; }
    }
}
