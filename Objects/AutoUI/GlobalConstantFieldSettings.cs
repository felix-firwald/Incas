using Incas.DialogSimpleForm.Components;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для GlobalsFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class GlobalConstantFieldSettings : BaseFieldSettings
    {
        #region Data

        [Description("Выбор константы из списка")]
        public Selector Selector { get; set; }
        #endregion
        public GlobalConstantFieldSettings(Field field)
        {
            this.Source = field;
            Parameter p = new();
            this.Selector = new([]);
            foreach (KeyValuePair<System.Guid, string> pair in p.GetConstantsDictionary())
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
    }
}
