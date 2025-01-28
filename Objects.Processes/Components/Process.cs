using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Engine;
using Incas.Objects.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Incas.Objects.Processes.Components
{
    public sealed class Process : IObject, IHasAuthor, IHasCreationDate, IHasPreset, ITerminable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<FieldData> Fields { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public ProcessData Data { get; set; }
        public Guid StatusId { get; set; }
        public bool Terminated { get; set; }
        public DateTime TerminatedDate { get; set; }
        public List<Guid> Contributors { get; set; }
        public Guid Preset { get; set; }

        public IObject Copy()
        {
            Process process = new()
            {
                Fields = this.Fields,
                OpenDate = this.OpenDate,
                CloseDate = this.CloseDate,
                Contributors = this.Contributors
            };
            return process;
        }

        public Dictionary<string, string> AddServiceFields(Dictionary<string, string> pairs)
        {
            Helpers.AppendAuthorServiceField(this, pairs);
            Helpers.AppendCreationDateServiceField(this, pairs);
            Helpers.AppendPresetServiceField(this, pairs);
            Helpers.AppendTerminableServiceFields(this, pairs);
            pairs.Add(Helpers.DateOpenedField, this.OpenDate.ToString());
            pairs.Add(Helpers.DateClosedField, this.CloseDate.ToString());
            pairs.Add(Helpers.ContributorsField, JsonConvert.SerializeObject(this.Contributors));
            pairs.Add(Helpers.DataField, JsonConvert.SerializeObject(this.Data));
            return pairs;
        }

        public void Initialize()
        {
            this.AuthorId = ProgramState.CurrentUser.id;
            this.CreationDate = DateTime.Now;
        }

        public void ParseServiceFields(DataRow dr)
        {
            Helpers.ParseAuthorServiceField(this, dr);
            Helpers.ParseCreationDateServiceField(this, dr);
            Helpers.ParsePresetServiceField(this, dr);
            Helpers.ParseTerminableServiceFields(this, dr);
            DateTime dtOpen;
            DateTime.TryParse(dr[Helpers.DateOpenedField].ToString(), out dtOpen);
            this.OpenDate = dtOpen;
            DateTime dtClose;
            DateTime.TryParse(dr[Helpers.DateClosedField].ToString(), out dtClose);
            this.CloseDate = dtClose;
            this.Data = JsonConvert.DeserializeObject<ProcessData>(dr[Helpers.DataField].ToString());
            this.Contributors = JsonConvert.DeserializeObject<List<Guid>>(dr[Helpers.ContributorsField].ToString());
        }
    }
}
