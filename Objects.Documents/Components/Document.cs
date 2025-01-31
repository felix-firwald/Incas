using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Objects.Documents.Components
{
    public sealed class Document : IObject, IHasAuthor, IHasCreationDate, IHasPreset, ITerminable
    {
        public IClass Class { get; set; }
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public List<FieldData> Fields { get; set; }
        public byte Status { get; set; }
        public bool Terminated { get; set; }
        public DateTime TerminatedDate { get; set; }
        public object Data { get; set; }
        public Guid Preset { get; set; }

        public Document(IClass @class)
        {
            this.Class = @class;
        }

        public IObject Copy()
        {
            Document doc = new(this.Class)
            {
                Fields = this.Fields,
                Preset = this.Preset
            };
            return doc;
        }

        public Dictionary<string, string> AddServiceFields(Dictionary<string, string> pairs)
        {
            Helpers.AppendAuthorServiceField(this, pairs);
            Helpers.AppendCreationDateServiceField(this, pairs);
            Helpers.AppendPresetServiceField(this, pairs);
            Helpers.AppendTerminableServiceFields(this, pairs);
            
            return pairs;
        }

        public void Initialize()
        {
            this.AuthorId = ProgramState.CurrentWorkspace.CurrentUser.Id;
            this.CreationDate = DateTime.Now;
        }

        public void ParseServiceFields(DataRow dr)
        {
            Helpers.ParseAuthorServiceField(this, dr);
            Helpers.ParseCreationDateServiceField(this, dr);
            Helpers.ParsePresetServiceField(this, dr);
            Helpers.ParseTerminableServiceFields(this, dr);
        }
    }
}
