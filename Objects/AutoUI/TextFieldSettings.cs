using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Incas.Objects.Models;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TextFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class TextFieldSettings : BaseFieldSettings
    {
        #region Data
        [Description("Значение по умолчанию")]
        [MaxLength(80)]
        [CanBeNull]
        public string Text { get; set; }

        #endregion
        public TextFieldSettings(Incas.Objects.Models.Field field)
        {
            this.Source = field;
            this.GetBaseData();
            this.Text = field.Value;
        }
        #region Functionality
        public override void Save()
        {
            this.SaveBaseData();
            this.Source.Value = this.Text;
        }
        #endregion
    }
}
