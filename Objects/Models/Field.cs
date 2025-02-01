using Incas.Objects.Components;
using Newtonsoft.Json;
using System;

namespace Incas.Objects.Models
{
    public class Field
    {
        [JsonProperty("f_i")]
        public Guid Id { get; set; }

        [JsonProperty("f_nv")]
        public string VisibleName { get; set; }

        [JsonProperty("f_ni")]
        public string Name { get; set; }

        [JsonProperty("f_t")]
        public FieldType Type { get; set; }

        [JsonProperty("f_v")]
        public string Value { get; set; }

        [JsonProperty("f_nn")]
        public bool NotNull { get; set; }

        [JsonProperty("f_uni")]
        public bool IsUnique { get; set; }

        [JsonProperty("f_pre")]
        public bool PresettingEnabled { get; set; }

        [JsonProperty("f_con")]
        public bool Confidential { get; set; }

        [JsonProperty("f_des")]
        public string Description { get; set; }

        [JsonProperty("f_act")]
        public string Action { get; set; }

        [JsonProperty("f_eve")]
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
