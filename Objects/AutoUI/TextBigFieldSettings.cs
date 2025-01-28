using Incas.Core.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [CanBeNull]
        [StringLength(1200)]
        public string Text { get; set; }

        [Description("Конфиденциально")]
        public bool Confidential { get; set; }
        #endregion

        public TextBigFieldSettings(Incas.Objects.Models.Field field)
        {
            this.Source = field;
            this.GetBaseData();
            this.Text = field.Value;
            this.Confidential = field.Confidential;
        }

        #region Functionality
        public override void Save()
        {
            this.SaveBaseData();
            this.Source.Value = this.Text;
            this.Source.Confidential = this.Confidential;
        }
        #endregion
    }
}
