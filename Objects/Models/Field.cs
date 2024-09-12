using Incas.Core.Classes;
using Incas.Templates.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Models
{
    public class Field
    {
        public Guid Id { get; set; }
        public string VisibleName { get; set; }
        public string Name { get; set; }
        public TagType Type { get; set; }
        public string Value { get; set; }
        public int OrderNumber { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }
        public void SetId()
        {
            if (this.Id == Guid.Empty)
            {
                this.Id = Guid.NewGuid();
            }
        }
        public CommandSettings GetCommand()
        {
            try
            {
                return JsonConvert.DeserializeObject<CommandSettings>(Cryptographer.DecryptString(this.Command));
            }
            catch (Exception)
            {
                return new();
            }
        }
        public void SaveCommand(CommandSettings cs)
        {
            this.Command = Cryptographer.EncryptString(JsonConvert.SerializeObject(cs));
        }
    }
}
