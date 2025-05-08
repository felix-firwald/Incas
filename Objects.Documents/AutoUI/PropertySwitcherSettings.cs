using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.DialogSimpleForm.Exceptions;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Incas.Objects.Documents.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TemplateSwitcher.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class PropertySwitcherSettings : AutoUIBase
    {
        #region Data
        private TemplateProperty property { get; set; }

        [Description("Поле для сравнения значения")]
        public Selector Selector { get; set; }

        [Description("Значение по-умолчанию")]
        [MaxLength(400)]
        public string DefaultValue { get; set; }

        [Description("Словарь")]
        public DataTable Cases { get; set; }

        #endregion

        public PropertySwitcherSettings(TemplateProperty prop, List<Field> fields)
        {
            this.property = prop;
            if (this.property.Switcher is null)
            {
                this.property.Switcher = new();
            }
            Dictionary<object, string> pairs = new();
            foreach (Field field in fields)
            {
                switch (field.Type)
                {
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.Integer:
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.Boolean:
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.LocalEnumeration:
                    case IncasEngine.ObjectiveEngine.Classes.FieldType.GlobalEnumeration:
                        pairs.Add(field.Id, field.Name);
                        break;
                }               
            }
            this.Selector = new(pairs);          
            this.Selector.SetSelection(prop.Switcher.TargetField);
            this.DefaultValue = prop.Switcher.Default;
            this.Cases = prop.Switcher.Cases.ToDataTable("Ожидаемый термин", "Вернуть определение");
        }

        #region Functionality
        public override void Load()
        {
            
        }

        public override void Validate()
        {
            if (this.Cases.Rows.Count == 0)
            {
                throw new SimpleFormFailed("Разветвитель не может не содержать ни единого сравнения.");
            }
        }

        public override void Save()
        {
            this.property.Switcher.TargetField = (Guid)this.Selector.SelectedObject;
            this.property.Switcher.Default = this.DefaultValue;
            this.property.Switcher.Cases = this.Cases.ToDictionary<string,string>("Ожидаемый термин", "Вернуть определение");
        }
        #endregion
    }
}
