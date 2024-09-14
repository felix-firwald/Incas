using Incas.Objects.Components;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для NumberFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class NumberFieldSettings : BaseFieldSettings
    {
        #region Data
        [Description("Минимальное значение")]
        public int MinValue { get; set; }

        [Description("Значение по-умолчанию")]
        public int DefaultValue { get; set; }

        [Description("Максимальное значение")]
        public int MaxValue { get; set; }
        #endregion

        public NumberFieldSettings(Incas.Objects.Models.Field field)
        {
            this.Source = field;
            this.GetBaseData();
            try
            {
                NumberFieldData nf = JsonConvert.DeserializeObject<NumberFieldData>(field.Value);
                this.MinValue = nf.MinValue;
                this.DefaultValue = nf.DefaultValue;
                this.MaxValue = nf.MaxValue;
            }
            catch
            {

            }
        }

        #region Functionality
        public void Save()
        {
            this.SaveBaseData();
            NumberFieldData nf = new()
            {
                MinValue = this.MinValue,
                MaxValue = this.MaxValue,
                DefaultValue = this.DefaultValue
            };
            this.Source.Value = JsonConvert.SerializeObject(nf);
        }
        #endregion
    }
}
