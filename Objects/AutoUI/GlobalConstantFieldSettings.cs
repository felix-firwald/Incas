using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using System.ComponentModel;
using Incas.Core.Models;
using AvaloniaEdit.Utils;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Bibliography;

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
        public GlobalConstantFieldSettings(Incas.Objects.Models.Field field)
        {
            this.Source = field;
            Parameter p = new();
            this.Selector = new(new());
            foreach (KeyValuePair<System.Guid,string> pair in p.GetConstantsDictionary())
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
        public override void Save()
        {
            this.SaveBaseData();
            this.Source.Value = this.Selector.SelectedObject.ToString();
        }
        #endregion
    }
}
