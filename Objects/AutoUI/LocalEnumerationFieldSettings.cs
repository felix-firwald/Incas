using Incas.Core.Classes;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для LocalEnumerationFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class LocalEnumerationFieldSettings : BaseFieldSettings
    {
        #region Data
        [Description("Значения перечисления")]
        public List<string> Values { get; set; }
        #endregion

        public LocalEnumerationFieldSettings(Field field)
        {
            this.Source = field;
            this.GetBaseData();
            try
            {
                this.Values = field.GetLocalEnumeration();
                //this.Values = JsonConvert.DeserializeObject<List<string>>(field.Value);
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
                this.Values = [];
            }
        }

        #region Functionality
        public override void Save()
        {
            this.SaveBaseData();
            //this.Source.Value = JsonConvert.SerializeObject(this.Values);
            this.Source.SetLocalEnumeration(this.Values);
        }
        #endregion
    }
}
