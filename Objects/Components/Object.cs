using System;
using System.Collections.Generic;

namespace Incas.Objects.Components
{
    public struct FieldData
    {
        public Guid ClassFieldId { get; set; }
        public string Value { get; set; }
    }
    public class Object
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid AuthorId { get; set; }
        public Guid Status { get; set; }
        public string Name { get; set; }
        public object Meta { get; set; }
        public List<FieldData> Fields { get; set; }
        public string GetFieldValue(Guid id)
        {
            foreach (FieldData field in this.Fields)
            {
                if (field.ClassFieldId == id)
                {
                    return field.Value;
                }
            }
            return "";
        }
    }
}
