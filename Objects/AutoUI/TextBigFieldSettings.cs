using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Incas.Objects.Models;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TextBigFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class TextBigFieldSettings : BaseFieldSettings
    {
        #region Data
        [Description("Значение по умолчанию")]
        [MaxLength(1200)]
        public string Text { get; set; }
        #endregion

        public TextBigFieldSettings(Incas.Objects.Models.Field field)
        {
            this.Source = field;
            this.GetBaseData();
            this.Text = field.Value;
        }

        #region Functionality
        public void Save()
        {
            this.SaveBaseData();
            this.Source.Value = this.Text;
        }
        #endregion
    }
}
