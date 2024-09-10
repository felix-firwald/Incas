using Incas.Core.Classes;
using Incas.Templates.Components;
using Newtonsoft.Json;
using System;

namespace Incas.Templates.Models
{
    public struct TagParameters
    {
        public string Name;
    }

    public class Tag
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string visibleName { get; set; }
        public TagType type { get; set; }
        public string value { get; set; }
        public int orderNumber { get; set; }
        public string description { get; set; }
        public string command { get; set; }
        public Tag()
        {
            
        }

        public void SetId()
        {
            if (this.id == Guid.Empty)
            {
                this.id = Guid.NewGuid();
            }
        }

        public CommandSettings GetCommand()
        {
            try
            {
                return JsonConvert.DeserializeObject<CommandSettings>(Cryptographer.DecryptString(this.command));
            }
            catch (Exception)
            {
                return new();
            }
        }
        public void SaveCommand(CommandSettings cs)
        {
            this.command = Cryptographer.EncryptString(JsonConvert.SerializeObject(cs));
        }
    }
}