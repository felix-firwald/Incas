using Incas.Objects.Components;
using Incas.Objects.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.StaticModels.Components
{
    public sealed class StaticObject : IObject
    {
        public IClass Class { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<FieldData> Fields { get; set; }

        public StaticObject(IClass @class)
        {
            this.Class = @class;
        }
        public IObject Copy()
        {
            StaticObject staticObject = new(this.Class)
            {
                Fields = this.Fields
            };
            return staticObject;
        }

        public Dictionary<string, string> AddServiceFields(Dictionary<string, string> result)
        {
            return result;
        }

        public void Initialize()
        {
            
        }

        public void ParseServiceFields(DataRow dr)
        {
            
        }
    }
}
