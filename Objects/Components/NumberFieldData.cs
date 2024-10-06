using Newtonsoft.Json;

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
