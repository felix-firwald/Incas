using Newtonsoft.Json;
using System;

namespace Incas.Objects.Components
{
    public struct DateFieldData
    {
        public DateFormats Format { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public static DateFieldData GetFromString(string value)
        {
            return JsonConvert.DeserializeObject<DateFieldData>(value);
        }
    }
}
