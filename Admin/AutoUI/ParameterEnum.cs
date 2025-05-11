using Incas.DialogSimpleForm.Components;
using IncasEngine.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Admin.AutoUI
{
    public class ParameterEnum : StaticAutoUIBase
    {
        protected override string FinishButtonText { get => "Сохранить перечисление"; }

        private bool isEdit = false;
        [Description("Наименование перечисления")]
        public string Name { get; set; }

        [Description("Значения перечисления")]
        public List<string> Value { get; set; }

        public ParameterEnum(bool isEdit = false)
        {
            this.isEdit = isEdit;
        }
        public override void Validate()
        {
            if (this.isEdit == false)
            {
                using Parameter p = new();
                foreach (string name in p.GetEnumeratorsList())
                {
                    if (name == this.Name)
                    {
                        throw new DialogSimpleForm.Exceptions.SimpleFormFailed("Глобальное перечисление с таким наименованием уже есть в рабочем пространстве.");
                    }
                }
            }           
        }
    }
}
