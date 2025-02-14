using Incas.DialogSimpleForm.Components;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.FieldComponents;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для GlobalEnumerationFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class GlobalEnumerationFieldSettings : BaseFieldSettings
    {
        #region Data
        private GlobalEnumerationData data;

        [Description("Выбор перечисления из списка")]
        public Selector Selector { get; set; }
        #endregion

        public GlobalEnumerationFieldSettings(Field field)
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
                this.data = field.GetGlobalEnumeration();
                this.Selector.SetSelection(this.data.TargetId);
            }
            catch
            {

            }
        }

        #region Functionality
        public override void Save()
        {
            this.SaveBaseData();
            this.data.TargetId = (System.Guid)this.Selector.SelectedObject;
            this.Source.SetGlobalEnumeration(this.data);
        }
        #endregion
    }
}
