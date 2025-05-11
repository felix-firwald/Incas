using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для DataSearchPredefined.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class DataSearchPredefined : StaticAutoUIBase
    {
        protected override string FinishButtonText { get => "Найти"; }
        #region Data
        private Field targetField { get; set; }

        [Description("Значение")]
        public string Value { get; set; }

        [Description("По полному совпадению")]
        public bool OnlyEqual { get; set; }
        #endregion

        public DataSearchPredefined(IClassData data, Guid targetField)
        {
            foreach (Field field in data.Fields)
            {
                if (field.Id == targetField)
                {
                    this.targetField = field;
                    break;
                }
                else
                {
                    this.targetField = new();
                }
            }           
        }

        #region Functionality
        public FieldData GetData()
        {
            FieldData f = new()
            {
                ClassField = this.targetField,
                Value = this.Value
            };
            return f;
        }
        #endregion
    }
}
