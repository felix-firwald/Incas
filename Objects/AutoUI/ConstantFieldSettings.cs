using Incas.Core.AutoUI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ConstantFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ConstantFieldSettings : AutoUIBase
    {
        #region Data
        [Description("Значение константы")]
        [MaxLength(1200)]
        public string Text { get; set; }
        #endregion

        #region Functionality

        #endregion
    }
}
