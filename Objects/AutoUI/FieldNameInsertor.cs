using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Models;
using System.Collections.Generic;
using System.ComponentModel;
using Field = Incas.Objects.Models.Field;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для FieldNameInsertor.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class FieldNameInsertor : AutoUIBase
    {
        #region Data
        [Description("Выбор поля")]
        public Selector Selector { get; set; }
        #endregion

        public FieldNameInsertor(List<Field> fields)
        {
            this.Selector = new(new());
            foreach (Field field in fields)
            {
                this.Selector.Pairs.Add(field, field.Name);
            }
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
            
        }
        public string GetSelectedField()
        {
            return "[" + this.Selector.SelectedValue + "]";
        }
        #endregion
    }
}
