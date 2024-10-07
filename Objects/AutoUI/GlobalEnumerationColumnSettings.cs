using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Components;
using System.ComponentModel;
using System.Collections.Generic;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для GlobalEnumerationColumnSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class GlobalEnumerationColumnSettings : AutoUIBase
    {
        #region Data
        private TableFieldColumnData Source;
        [Description("Выбор перечисления из списка")]
        public Selector Selector { get; set; }
        #endregion

        public GlobalEnumerationColumnSettings(TableFieldColumnData field)
        {
            this.Source = field;
            Parameter p = new();
            this.Selector = new([]);
            foreach (KeyValuePair<System.Guid, string> pair in p.GetEnumerationsDictionary())
            {
                this.Selector.Pairs.Add(pair.Key, pair.Value);
            }
            try
            {
                this.Selector.SetSelection(System.Guid.Parse(field.Value));
            }
            catch
            {

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
            this.Source.Value = this.Selector.SelectedObject.ToString();
        }
        #endregion
    }
}
