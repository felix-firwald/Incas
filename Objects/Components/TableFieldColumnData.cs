using IncasEngine.ObjectiveEngine.Classes;

namespace Incas.Objects.Components
{
    public class TableFieldColumnData
    {
        public string Name { get; set; }
        public string VisibleName { get; set; }
        public FieldType FieldType { get; set; }
        public string Value { get; set; }
        public bool NotNull { get; set; }
    }
}
