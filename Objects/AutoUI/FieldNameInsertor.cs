using Incas.DialogSimpleForm.Components;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Models;
using System.Collections.Generic;
using System.ComponentModel;

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

        public FieldNameInsertor(List<FieldViewModel> fields)
        {
            this.Selector = new(new());
            foreach (FieldViewModel field in fields)
            {
                switch (field.Type)
                {
                    case FieldType.String:
                    case FieldType.Text:
                    case FieldType.Integer:
                    case FieldType.Date:
                    case FieldType.Object:
                        this.Selector.Pairs.Add(field, field.Name);
                        break;
                }               
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
