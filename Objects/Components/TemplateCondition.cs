using System;

namespace Incas.Objects.Components
{
    public class TemplateCondition
    {
        public Guid Field { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public string TargetValue { get; set; }
    }
}
