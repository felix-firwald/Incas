using Incas.Core.Classes;
using Incas.Objects.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Incas.Objects.Components
{
    public class Preset
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid AuthorId { get; set; }
        public string Name { get; set; }
        public Dictionary<Guid,string> Data { get; set; }
        private List<FieldData> PresettingFields { get; set; }
        public Preset()
        {
            this.PresettingFields = new();
        }
        public void SetData(string data)
        {
            this.Data = JsonConvert.DeserializeObject<Dictionary<Guid,string>>(data);
        }
        public string GetData()
        {
            return JsonConvert.SerializeObject(this.Data);
        }
        public List<FieldData> GetPresettingData()
        {
            return this.PresettingFields;
        }
        public void RegisterPresettingField(Incas.Objects.Models.Field field, string value)
        {
            FieldData fd = new()
            {
                ClassField = field,
                Value = value
            };
            this.PresettingFields.Add(fd);
        }
        public PresetReference GetAsReference()
        {
            return new()
            {
                Id = this.Id,
                Name = this.Name
            };
        }
        public Dictionary<string,string> GetValues()
        {
            Dictionary<string, string> values = new();
            foreach (KeyValuePair<Guid, string> field in this.Data)
            {
                values.Add(field.Key.ToString(), field.Value);
            }
            return values;
        }
    }
}
