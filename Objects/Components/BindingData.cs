using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    public struct BindingData
    {
        public Guid Class { get; set; }
        public Guid Field { get; set; }
        public static BindingData GetFromString(string value)
        {
            return JsonConvert.DeserializeObject<BindingData>(value);
        }
    }
}
