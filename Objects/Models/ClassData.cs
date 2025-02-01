using Incas.Objects.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Incas.Objects.Models
{
    public class ClassData
    {
        [JsonProperty("c_f")]
        public List<Field> Fields { get; set; }

        [JsonProperty("c_nt")]
        public string NameTemplate { get; set; }

        [JsonProperty("c_sc")]
        public bool ShowCard { get; set; }

        [JsonProperty("c_pe")]
        public bool PresetsEnabled { get; set; }

        [JsonProperty("c_rfv")]
        public bool RestrictFullView { get; set; }

        [JsonProperty("c_ebao")]
        public bool EditByAuthorOnly { get; set; }

        [JsonProperty("c_aoe")]
        public bool AuthorOverrideEnabled { get; set; }

        [JsonProperty("c_s")]
        public Dictionary<int, StatusData> Statuses { get; set; }

        [JsonProperty("c_t")]
        public Dictionary<int, TemplateData> Templates { get; set; }

        [JsonProperty("c_itn")]
        public bool InsertTemplateName { get; set; }

        [JsonProperty("c_pys")]
        public string Script { get; set; }
        public void AddStatus(StatusData data)
        {
            this.Statuses ??= [];
            this.Statuses.Add(this.Statuses.Count + 1, data);
        }
        public void RemoveStatus(int index)
        {
            this.Statuses.Remove(index);
            this.RecalculateIndexesOfStatuses();
        }
        public void AddTemplate(TemplateData data)
        {
            this.Templates ??= [];
            this.Templates.Add(this.Templates.Count + 1, data);
        }

        public void EditTemplate(int index, TemplateData data)
        {
            try
            {
                this.Templates[index] = data;
            }
            catch { }
        }
        public void RemoveTemplate(int index)
        {
            this.Templates.Remove(index);
            this.RecalculateIndexesOfTemplates();
        }
        private void RecalculateIndexesOfStatuses()
        {
            int index = 1;
            Dictionary<int, StatusData> result = new();
            foreach (KeyValuePair<int, StatusData> item in this.Statuses)
            {
                result.Add(index, item.Value);
                index += 1;
            }
            this.Statuses = result;
        }
        private void RecalculateIndexesOfTemplates()
        {
            int index = 1;
            Dictionary<int, TemplateData> result = new();
            foreach (KeyValuePair<int, TemplateData> item in this.Templates)
            {
                result.Add(index, item.Value);
                index += 1;
            }
            this.Templates = result;
        }
        public List<Field> GetFieldsForMap()
        {
            List<Field> list = [];
            if (this.Fields is null)
            {
                return list;
            }
            foreach (Field field in this.Fields)
            {
                switch (field.Type)
                {
                    case FieldType.Variable:
                    case FieldType.Text:
                    case FieldType.Number:
                    case FieldType.Relation:
                    case FieldType.LocalEnumeration:
                    case FieldType.GlobalEnumeration:
                    case FieldType.Date:
                        list.Add(field);
                        break;
                }
            }
            return list;
        }
        public List<Field> GetBindableFields()
        {
            List<Field> list = [];
            if (this.Fields is null)
            {
                return list;
            }
            foreach (Field field in this.Fields)
            {
                switch (field.Type)
                {
                    case FieldType.Variable:
                    case FieldType.Text:
                    case FieldType.Number:
                    case FieldType.Relation:
                    case FieldType.LocalEnumeration:
                    case FieldType.GlobalEnumeration:
                    case FieldType.Date:
                        list.Add(field);
                        break;
                }
            }
            return list;
        }
        public List<Field> GetPresettingFields()
        {
            List<Field> list = [];
            if (this.Fields is null)
            {
                return list;
            }
            foreach (Field field in this.Fields)
            {
                if (field.PresettingEnabled)
                {
                    list.Add(field);
                }              
            }
            return list;
        }
        public List<Field> GetSavebleFields()
        {
            List<Field> list = [];
            foreach (Field field in this.Fields)
            {
                switch (field.Type)
                {
                    default:
                        list.Add(field);
                        break;
                    case FieldType.LocalConstant:
                    case FieldType.GlobalConstant:
                    case FieldType.HiddenField:
                    case FieldType.Generator:
                        break;
                }
            }
            return list;
        }
        public Field FindFieldById(Guid id)
        {
            foreach (Field field in this.Fields)
            {
                if (field.Id == id)
                {
                    return field;
                }
            }
            return new();
        }
        public Field FindFieldByBackReference(Guid reference)
        {
            foreach (Field field in this.Fields)
            {
                if (field.Type == FieldType.Relation)
                {
                    BindingData bd = JsonConvert.DeserializeObject<BindingData>(field.Value);
                    if (bd.Class == reference)
                    {

                        return field;
                    }
                }
            }
            return new();
        }
        public Field FindFieldByName(string name)
        {
            foreach (Field field in this.Fields)
            {
                if (field.Name == name)
                {
                    return field;
                }
            }
            return new();
        }
        public Field FindFieldByVisibleName(string name)
        {
            foreach (Field field in this.Fields)
            {
                if (field.VisibleName == name)
                {
                    return field;
                }
            }
            return new();
        }
    }
}
