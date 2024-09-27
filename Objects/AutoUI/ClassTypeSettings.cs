using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Objects.Components;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ClassTypeSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ClassTypeSettings : AutoUIBase
    {
        #region Data
        [Description("Категория класса")]
        public string Category { get; set; }
        [Description("Имя класса")]
        public string Name { get; set; }
        [Description("Тип класса")]
        public ComboSelector Selector { get; set; }
        #endregion

        public ClassTypeSettings()
        {
            Dictionary<object, string> pairs = new()
            {
                { ClassType.Model, "Модель данных" },
                { ClassType.Document, "Документ" }
            };
            this.Selector = new(pairs);
        }

        #region Functionality

        #endregion
    }
}
