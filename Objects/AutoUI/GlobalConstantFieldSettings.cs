using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using System.ComponentModel;
using Incas.Core.Models;
using AvaloniaEdit.Utils;
using System.Collections.Generic;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для GlobalsFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class GlobalConstantFieldSettings : AutoUIBase
    {
        #region Data
        [Description("Выбор константы из списка")]
        public ComboSelector Selector { get; set; }
        #endregion
        public GlobalConstantFieldSettings()
        {
            Parameter p = new();
            this.Selector = new(new());
            foreach (KeyValuePair<System.Guid,string> pair in p.GetConstantsDictionary())
            {
                this.Selector.Pairs.Add(pair.Key, pair.Value);
            }
        }
        #region Functionality

        #endregion
    }
}
