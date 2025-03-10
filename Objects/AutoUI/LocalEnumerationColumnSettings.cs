using Incas.DialogSimpleForm.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using static IncasEngine.ObjectiveEngine.FieldComponents.TableFieldData;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для LocalEnumerationColumnSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class LocalEnumerationColumnSettings : AutoUIBase
    {
        #region Data
        private TableFieldColumnData Source;
        [Description("Значения перечисления")]
        public List<string> Values { get; set; }

        [Description("Обязательно для заполнения")]
        public bool NotNull { get; set; }
        #endregion

        public LocalEnumerationColumnSettings(TableFieldColumnData field)
        {
            this.Source = field;
            this.NotNull = field.NotNull;
            try
            {
                this.Values = JsonConvert.DeserializeObject<List<string>>(field.Value);
            }
            catch
            {
                this.Values = [];
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
            this.Source.Value = JsonConvert.SerializeObject(this.Values);
            this.Source.NotNull = this.NotNull;
        }
        #endregion
    }
}
