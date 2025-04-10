using Incas.Core.Attributes;
using Incas.DialogSimpleForm.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WinRT;

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

        [Description("Маска валидации")]
        [MaxLength(80)]
        [CanBeNull]
        public string Mask { get; set; }

        [Description("Минимальная длина")]
        public int MinLength { get; set; }

        [Description("Максимальная длина")]
        public int MaxLength { get; set; }

        [Description("Конфиденциально")]
        public bool Confidential { get; set; }

        #endregion
        public TextFieldSettings(Field field)
        {
            this.Source = field;
            this.GetBaseData();
            this.Text = field.Value;
            this.MinLength = field.MinLength;
            this.MaxLength = field.MaxLength;
            this.Mask = field.Mask;
            this.Confidential = field.Confidential;
        }
        #region Functionality
        public override void Validate()
        {
            if (this.MinLength < 0 || this.MaxLength < 0)
            {
                throw new SimpleFormFailed("Длина поля не может быть меньше нуля.");
            }
            if (this.MinLength > 120 || this.MaxLength > 120)
            {
                throw new SimpleFormFailed("Длина поля этого типа не может быть более 120 знаков.");
            }
        }
        public override void Save()
        {
            this.SaveBaseData();
            this.Source.Value = this.Text;
            if (this.MinLength > this.MaxLength)
            {
                this.MinLength = this.MaxLength;
            }
            this.Source.Confidential = this.Confidential;
            this.Source.MinLength = this.MinLength;
            this.Source.MaxLength = this.MaxLength;
            if (string.IsNullOrEmpty(this.Mask))
            {
                this.Mask = null;
            }
            this.Source.Mask = this.Mask;
        }
        #endregion
    }
}
