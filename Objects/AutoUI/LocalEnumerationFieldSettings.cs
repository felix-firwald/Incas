using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для LocalEnumerationFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class LocalEnumerationFieldSettings : BaseFieldSettings
    {
        #region Data

        [Description("Значения перечисления")]
        public List<string> Values { get; set; }
        #endregion

        #region Functionality

        #endregion
    }
}
