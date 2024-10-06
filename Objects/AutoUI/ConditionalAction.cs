using Incas.Core.Attributes;
using Incas.DialogSimpleForm.Components;
using Incas.Core.Classes;
using Incas.Objects.Components;
using System.Collections.Generic;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ConditionalAction.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ConditionalAction : AutoUIBase
    {
        #region Data
        [Description("Выбор поля")]
        public Selector FieldSelector { get; set; }

        [Description("Вид сравнения")]
        public Selector ComparisonSelector { get; set; }

        [CanBeNull]
        [Description("Значение")]
        public string TargetValue { get; set; }

        [Description("Замена при положительной проверке")]
        public Dictionary<string, string> TrueAction { get; set; }

        [Description("Замена при отрицательной проверке")]
        public Dictionary<string, string> FalseAction { get; set; }
        #endregion

        public ConditionalAction()
        {
            this.ComparisonSelector = new([])
            {
                Pairs = new()
            {
                { ComparisonType.Equal, "равно" },
                { ComparisonType.NotEqual, "не равно" },
                { ComparisonType.Contains, "содержит" },
                { ComparisonType.NotContains, "не содержит" },
            }
            };

        }

        #region Functionality

        #endregion
    }
}
