using Incas.Objects.Interfaces;
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
    public class Object : IObject
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid AuthorId { get; set; }
        public byte Status { get; set; }
        public string Name { get; set; }
        public bool Terminated { get; set; }
        public DateTime TerminatedDate { get; set; }
        public Guid TargetClass { get; set; }
        public Guid TargetObject { get; set; }
        public object Meta { get; set; }
        public List<FieldData> Fields { get; set; }
        public Object Copy()
        {
            Object obj = new()
            {
                Fields = this.Fields
            };
            return obj;
        }
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
