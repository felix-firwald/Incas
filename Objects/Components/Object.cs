using Incas.Objects.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    public sealed class Object : IObject, IHasAuthor, IHasPreset // only for data models (simple objects)
    {
        public IClass Class { get; set; }
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Name { get; set; }
        public List<FieldData> Fields { get; set; }
        public bool Terminated { get; set; }
        public DateTime TerminatedDate { get; set; }
        public object Data { get; set; }
        public Guid Preset { get; set; }

        public Object(IClass @class)
        {
            this.Class = @class;
        }
        public IObject Copy()
        {
            Object obj = new(this.Class)
            {
                Fields = this.Fields,
                Preset = this.Preset
            };
            return obj;
        }

        public Dictionary<string, string> AddServiceFields(Dictionary<string, string> result)
        {
            Helpers.AppendAuthorServiceField(this, result);
            Helpers.AppendPresetServiceField(this, result);
            return result;
        }

        public void Initialize()
        {
            
        }

        public void ParseServiceFields(DataRow dr)
        {
            Helpers.ParseAuthorServiceField(this, dr);
            Helpers.ParsePresetServiceField(this, dr);
        }
    }
}
