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

        public override void Validate()
        {
            using (Parameter p = new())
            {
                foreach (string name in p.GetEnumeratorsList())
                {
                    if (name == this.Name)
                    {
                        throw new Core.Exceptions.SimpleFormFailed("Глобальное перечисление с таким наименованием уже есть в рабочем пространстве.");
                    }
                }
            }
        }
    }
}
