using Incas.Core.Attributes;
using IncasEngine.ObjectiveEngine.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [Description("Должно быть уникальным")]
        public bool Unique { get; set; }

        [Description("Значение по умолчанию")]
        [MaxLength(80)]
        [CanBeNull]
        public string Text { get; set; }

        [Description("Конфиденциально")]
        public bool Confidential { get; set; }

        #endregion
        public TextFieldSettings(Field field)
        {
            this.Source = field;
            this.GetBaseData();
            this.Text = field.Value;
            this.Unique = field.IsUnique;
            this.Confidential = field.Confidential;
        }
        #region Functionality
        public override void Validate()
        {
            if (this.Unique && this.NotNull == false)
            {
                throw new DialogSimpleForm.Exceptions.SimpleFormFailed("Поле не может быть пустым, если требуется уникальность.");
            }
        }
        public override void Save()
        {
            this.SaveBaseData();
            this.Source.Value = this.Text;
            this.Source.IsUnique = this.Unique;
            this.Source.Confidential = this.Confidential;
        }
        #endregion
    }
}
