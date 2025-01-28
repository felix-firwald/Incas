using Incas.Objects.Components;
using Newtonsoft.Json;
using System;

namespace Incas.Objects.Models
{
    public class Field
    {
        public Guid Id { get; set; }
        public string VisibleName { get; set; }
        public string Name { get; set; }
        public FieldType Type { get; set; }
        public string Value { get; set; }
        public bool NotNull { get; set; }
        public bool IsUnique { get; set; }
        public bool PresettingEnabled { get; set; }
        public bool Confidential { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
        public string ChangedEvent { get; set; }
        public void SetId()
        {
            if (this.Id == Guid.Empty)
            {
                this.Id = Guid.NewGuid();
            }
            if (string.IsNullOrWhiteSpace(this.VisibleName))
            {
                this.VisibleName = this.Name.Replace("_", " ");
            }
        }
        public BindingData GetBindingData()
        {
            return JsonConvert.DeserializeObject<BindingData>(this.Value);
        }
        public DateFieldData GetDateFieldData()
        {
            return JsonConvert.DeserializeObject<DateFieldData>(this.Value);
        }
        public NumberFieldData GetNumberFieldData()
        {
            return JsonConvert.DeserializeObject<NumberFieldData>(this.Value);
        }
    }
}
