using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Management.Deployment;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ClassFieldsPresetting.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ClassFieldsPresetting : AutoUIBase
    {
        #region Data
        private readonly List<Models.Field> fields;

        [Description("Ограничить просмотр общего списка")]
        public bool Constraint { get; set; }
        [Description("Поля, участвующие в пресетах")]
        public CheckedList Fields { get; set; }
        #endregion

        public ClassFieldsPresetting(List<Models.Field> fields, bool constraint)
        {
            this.fields = fields;
            Dictionary<CheckedItem, bool> pairs = new();
            foreach (Models.Field field in fields)
            {
                CheckedItem ci = new()
                {
                    Target = field,
                    Name = field.Name
                };
                pairs.Add(ci, field.PresettingEnabled);
            }
            this.Fields = new(pairs);
            this.Constraint = constraint;
        }

        #region Functionality
        public override void Load()
        {
            
        }

        public override void Validate()
        {

        }

        public override void Save()
        {
            foreach (Models.Field field in this.fields)
            {
                foreach (KeyValuePair<CheckedItem, bool> pair in this.Fields.Pairs)
                {
                    if (field == pair.Key.Target)
                    {
                        field.PresettingEnabled = pair.Value;
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
