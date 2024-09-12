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
        public string Comment { get; set; }
        public object Meta { get; set; }
        public List<FieldData> Fields { get; set; }
    }
}
