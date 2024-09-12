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

        #region Functionality

        #endregion
    }
}
