using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.Core.AutoUI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для BaseFieldSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class BaseFieldSettings : AutoUIBase
    {
        #region Data
        [MaxLength(200)]
        [Description("Описание поля (для форм)")]
        public string Description { get; set; }

        [Description("Обязательно для заполнения")]
        public bool NotNull { get; set; }
        

        #endregion

        #region Functionality

        #endregion
    }
}
