using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    public struct NumberFieldData
    {
        public int MinValue { get; set; }
        public int DefaultValue { get; set; }
        public int MaxValue { get; set; }
        public static NumberFieldData GetFromString(string value)
        {
            return JsonConvert.DeserializeObject<NumberFieldData>(value);
        }
    }
}
