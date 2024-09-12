using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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

        #region Functionality

        #endregion
    }
}
