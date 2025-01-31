using Incas.Objects.Components;
using Newtonsoft.Json;
using System;

namespace Incas.Objects.Models
{
    public class Field
    {
        [JsonProperty("i")]
        public Guid Id { get; set; }

        [JsonProperty("nv")]
        public string VisibleName { get; set; }

        [JsonProperty("ni")]
        public string Name { get; set; }

        [JsonProperty("t")]
        public FieldType Type { get; set; }

        [JsonProperty("v")]
        public string Value { get; set; }

        [JsonProperty("nn")]
        public bool NotNull { get; set; }

        [JsonProperty("uniq")]
        public bool IsUnique { get; set; }

        [JsonProperty("pres")]
        public bool PresettingEnabled { get; set; }

        [JsonProperty("conf")]
        public bool Confidential { get; set; }

        [JsonProperty("d")]
        public string Description { get; set; }

        [JsonProperty("act")]
        public string Action { get; set; }

        [JsonProperty("event")]
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
