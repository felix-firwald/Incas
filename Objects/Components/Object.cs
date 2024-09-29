using Incas.Objects.Models;
using System;
using System.Collections.Generic;

namespace Incas.Objects.Components
{
    public struct FieldData
    {
        public Field ClassField { get; set; }
        public string Value { get; set; }
    }
    public class Object
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid AuthorId { get; set; }
        public byte Status { get; set; }
        public string Name { get; set; }
        public object Meta { get; set; }
        public List<FieldData> Fields { get; set; }
        public string GetFieldValue(Guid id)
        {
            foreach (FieldData field in this.Fields)
            {
                if (field.ClassField.Id == id)
                {
                    return field.Value;
                }
            }
            return "";
        }
    }
}
