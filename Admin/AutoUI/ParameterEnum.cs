using System.ComponentModel;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using Incas.Core.Models;
using Incas.Core.AutoUI;

namespace Incas.Admin.AutoUI
{
    public class ParameterEnum : AutoUIBase
    {
        [Description("Наименование перечисления")]
        public string Name { get; set; }

        [Description("Значения перечисления")]
        public List<string> Value { get; set; }
    }
}
